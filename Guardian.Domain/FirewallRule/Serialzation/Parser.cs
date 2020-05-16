using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Guardian.Domain.FirewallRule.Serialzation
{
    public class Parser : IParser
    {
        private string[] xDirectives = new string[] { "SecAction", "SecArgumentSeparator", "SecAuditEngine", "SecAuditLog", "SecAuditLog2", "SecAuditLogDirMode", "SecAuditLogFormat", "SecAuditLogFileMode", "SecAuditLogParts", "SecAuditLogRelevantStatus", "SecAuditLogStorageDir", "SecAuditLogType", "SecCacheTransformations", "SecChrootDir", "SecCollectionTimeout", "SecComponentSignature", "SecConnEngine", "SecContentInjection", "SecCookieFormat", "SecCookieV0Separator", "SecDataDir", "SecDebugLog", "SecDebugLogLevel", "SecDefaultAction", "SecDisableBackendCompression", "SecHashEngine", "SecHashKey", "SecHashParam", "SecHashMethodRx", "SecHashMethodPm", "SecGeoLookupDb", "SecGsbLookupDb", "SecGuardianLog", "SecHttpBlKey", "SecInterceptOnError", "SecMarker", "SecPcreMatchLimit", "SecPcreMatchLimitRecursion", "SecPdfProtect", "SecPdfProtectMethod", "SecPdfProtectSecret", "SecPdfProtectTimeout", "SecPdfProtectTokenName", "SecReadStateLimit", "SecConnReadStateLimit", "SecSensorId", "SecWriteStateLimit", "SecConnWriteStateLimit", "SecRemoteRules", "SecRemoteRulesFailAction", "SecRequestBodyAccess", "SecRequestBodyInMemoryLimit", "SecRequestBodyLimit", "SecRequestBodyNoFilesLimit", "SecRequestBodyLimitAction", "SecResponseBodyLimit", "SecResponseBodyLimitAction", "SecResponseBodyMimeType", "SecResponseBodyMimeTypesClear", "SecResponseBodyAccess", "SecRuleInheritance", "SecRuleEngine", "SecRulePerfTime", "SecRuleRemoveById", "SecRuleRemoveByMsg", "SecRuleRemoveByTag", "SecRuleScript", "SecRuleUpdateActionById", "SecRuleUpdateTargetById", "SecRuleUpdateTargetByMsg", "SecRuleUpdateTargetByTag", "SecServerSignature", "SecStatusEngine", "SecStreamInBodyInspection", "SecStreamOutBodyInspection", "SecTmpDir", "SecUnicodeMapFile", "SecUnicodeCodePage", "SecUploadDir", "SecUploadFileLimit", "SecUploadFileMode", "SecUploadKeepFiles", "SecWebAppId", "SecXmlExternalEntity" };
        private readonly ILogger<Parser> _logger;

        public Parser(ILogger<Parser> logger)
        {
            _logger = logger;
        }
        public (List<Rule>, bool) GetRules(string raw)
        {
            try
            {
                var plainTextRules = new List<string>();
                var sr = new StringReader(raw);
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Length == 1 || line.StartsWith("#"))
                    {
                        continue;
                    }
                    var readLine = line.Trim(' ').TrimEnd('\r').Replace(Environment.NewLine, " ");

                    if (readLine.Length <= 1)
                    {
                        continue;
                    }
                    plainTextRules.Add(readLine);
                }

                var plainTextRulesLen = plainTextRules.Count;

                var parseResult = new List<Rule>();
                for (var i = 0; i < plainTextRulesLen; i++)
                {
                    var row = plainTextRules[i];
                    if (row.StartsWith("SecRule"))
                    {
                        Rule rule;

                        (rule, i) = Walk(plainTextRules, i, plainTextRulesLen);
                        parseResult.Add(rule);
                    }
                }

                return (parseResult, true);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Rule parse failed!");

                return (null, false);
            }
        }

        private (Rule, int) Walk(List<string> plainTextRules, int i, int plainTextRulesLen)
        {
            var row = plainTextRules[i];
            Rule chainRule = null;

            var chainWait = false;
            while (true)
            {
                var li = i+1;

                if (li >= plainTextRulesLen)
                {
                    break;
                }
                var lr = plainTextRules[li];

                if (lr.StartsWith("chain") || lr.StartsWith("\"chain"))
                {
                    chainWait = true;
                }
                if (lr.StartsWith("SecRule"))
                {
                    if (chainWait)
                    {
                        (chainRule, i) = Walk(plainTextRules, li, plainTextRulesLen);
                    }
                    break;
                }
                var isDirective = false;
                foreach (var dir in xDirectives)
                {
                    if (lr.StartsWith(dir))
                    {
                        isDirective = true;
                        break;
                    }
                }

                if (isDirective)
                {
                    break;
                }
                i = li;
                row += lr;
            }


            var rule = ParseRule(row);

            if (chainRule != null)
            {
                rule.Chain = chainRule;
            }
            return (rule, i);
        }

        private Rule ParseRule(string ruleTxt)
        {
            var variablesReg = new Regex("SecRule\\s(.*?)\\s", RegexOptions.Compiled);
            var operatorReg = new Regex("(\\\"@?.*?\\\")", RegexOptions.Compiled);

            var variablesMatch = variablesReg.Match(ruleTxt);
            var operatorMatch = operatorReg.Match(ruleTxt);

            if (variablesMatch.Value == "")
            {
                return null;
            }
            var variables = ParseVariables(variablesMatch.Value);
            var operators = ParseOperators(operatorMatch.Value);
            var action = ParseAction(ruleTxt.Replace(operatorMatch.Value, "").Replace(variablesMatch.Value, ""));

            return new Rule { Variables = variables, Operator = operators, Action = action };
        }

        private List<Variable> ParseVariables(string variable)
        {
            variable = variable.Replace("SecRule ", "");

            var varsSplit = variable.Split('|');
            var dataVariable = new List<Variable>();


            foreach (var vars in varsSplit)
            {
                var varsAndFilter = vars.Split(":");
                if (varsAndFilter.Length > 2)
                {
                    //TODO Malformed rule
                    continue;
                }

                Variable v;

                var isLengthCheck = varsAndFilter[0][0] == '&';
                if (varsAndFilter.Length > 1)
                {
                    var isNotType = varsAndFilter[0][0] == '!';
                    var varName = varsAndFilter[0].Trim(' ');

                    if (isNotType || isLengthCheck)
                    {
                        varName = varName.Substring(1);
                    }

                    v = new Variable { Name = varName, Filter = varsAndFilter[1].Trim(' ').Split(",").ToList(), FilterIsNotType = isNotType, LengthCheckForCollection = isLengthCheck };
                }
                else
                {
                    var varName = varsAndFilter[0].Trim(' ');

                    if (isLengthCheck)
                    {
                        varName = varName.Substring(1);
                    }
                    v = new Variable { Name = varName, FilterIsNotType = false, LengthCheckForCollection = isLengthCheck };
                }

                dataVariable.Add(v);
            }

            return dataVariable;
        }

        private Operator ParseOperators(string @operator)
        {
            var isNotOperator = @operator.StartsWith("\"!");
            bool isOperatorSpec;

            if (isNotOperator)
            {
                isOperatorSpec = @operator.StartsWith("\"!@");
            }
            else
            {
                isOperatorSpec = @operator.StartsWith("\"@");
            }
            var parsedOperator = "rx";
            string parsedExpression;

            if (isOperatorSpec)
            {
                var operatorReg = new Regex("@(.*?)(\\s|\\\")", RegexOptions.Compiled);
                var opMatch = operatorReg.Matches(@operator);

                parsedOperator = opMatch[0].Groups[1].Value.Replace("\"", "");

                parsedExpression = @operator.Replace(parsedOperator, "");
                parsedExpression = parsedExpression.Trim('"', ' ', '\\').TrimStart('@', '!', ' ');
            }
            else
            {
                parsedExpression = @operator.Trim('"', ' ', '\\').TrimStart('@', '!', ' ');
            }

            return new Operator
            {
                Func = parsedOperator,
                Expression = parsedExpression,
                OperatorIsNotType = isNotOperator
            };
        }

        private Action ParseAction(string action)
        {

            var idReg = new Regex("id:(.*?),", RegexOptions.Compiled);
            var phaseReg = new Regex("phase:(.*?),", RegexOptions.Compiled);
            var transformReg = new Regex("t:(.*?),", RegexOptions.Compiled);

            var idRegMatch = idReg.Matches(action);
            var idRegIdentified = "-1";

            if (idRegMatch.Count > 0)
            {
                idRegIdentified = idRegMatch.ToList()[0].Groups[1].Value;
            }
            var phaseRegMatch = phaseReg.Matches(action);
            var phaseRegIdentified = 1;

            if (phaseRegMatch.Count > 0)
            {
                phaseRegIdentified = int.Parse(phaseRegMatch.ToList()[0].Groups[1].Value);
            }

            var disrupAct = DisruptiveAction.Block;

            if (action.Contains(DisruptiveAction.Pass.ToString().ToLower() + ","))
            {
                disrupAct = DisruptiveAction.Pass;
            }
            else if (action.Contains(DisruptiveAction.Drop.ToString().ToLower() + ","))
            {
                disrupAct = DisruptiveAction.Drop;
            }
            else if (action.Contains(DisruptiveAction.Deny.ToString().ToLower() + ","))
            {
                disrupAct = DisruptiveAction.Deny;
            }
            else if (action.Contains(DisruptiveAction.Proxy.ToString().ToLower() + ","))
            {
                disrupAct = DisruptiveAction.Proxy;
            }

            var transformMatch = transformReg.Matches(action);
            var transforms = new List<string>();

            if (transformMatch.Count > 0)
            {
                foreach (Match item in transformMatch)
                {
                    if (item.Length > 1)
                    {
                        transforms.Add(item.Groups[1].Value);
                    }
                }

            }
            var result = new Action
            {
                ID = idRegIdentified,
                Phase = phaseRegIdentified - 1,
                Transformations = transforms,
                DisruptiveAction = disrupAct,
                LogAction = 0
            };

            return result;
        }
    }
}
