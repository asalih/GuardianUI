using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Guardian.Domain
{
    public static class SSLHelper
    {
        private const string winCmd = "req -newkey rsa:2048 -x509 -nodes -keyout {2}\\{1}.key -new -out {2}\\{1}.crt -subj \"/CN={0}\" -reqexts SAN -extensions SAN -config \"{3}\" -sha256 -days 3650";
        private const string linuxCmd = "req \\ " +
"- newkey rsa:2048 \\" +
"-x509 \\" +
"-nodes \\" +
"-keyout {2}\\{1}.key \\" +
"-new \\" +
"-out {2}\\{1}.crt \\" +
"-subj /CN={0} \\" +
"-reqexts SAN \\" +
"-extensions SAN \\" +
"-config {3} \\" +
"-sha256 \\" +
"-days 3650";

        public static SSL CreateSSL(string domain)
        {
            return Infrastructure.OperatingSystem.IsWindows() ?
                CreateSelfSignedSSLForWin(domain) :
                CreateSelfSignedSSLForLinux(domain);
        }

        private static SSL CreateSelfSignedSSLForWin(string domain)
        {
            var openSSLDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "openssl");

            var execPath = Path.Combine(openSSLDir, "openssl.exe");

            var baseFileName = Guid.NewGuid().ToString("N");

            //Load base config file, append needed data and save for later usage
            var configFilePath = Path.Combine(openSSLDir, $"{baseFileName}.cnf");
            var configFileContent = File.ReadAllText(Path.Combine(openSSLDir, "openssl.cnf")).Replace("{dir_placeholder}", openSSLDir + "\\ssl") +
                    $"[SAN]\nsubjectAltName=DNS:{domain}";

            File.WriteAllText(configFilePath, configFileContent);

            var args = string.Format(winCmd, domain, baseFileName, openSSLDir, configFilePath);

            var psi = new Process
            {
                StartInfo = new ProcessStartInfo(execPath, args)
                {
                    UseShellExecute = true,
                }
            };
            psi.Start();

            psi.WaitForExit();

            var certCrtPath = Path.Combine(openSSLDir, $"{baseFileName}.crt");
            var certKeyPath = Path.Combine(openSSLDir, $"{baseFileName}.key");

            var result = new SSL()
            {
                CertCrt = File.ReadAllText(certCrtPath),
                CertKey = File.ReadAllText(certKeyPath)
            };

            InstallCertificate(certCrtPath);

            //Lets clear the path.
            File.Delete(certCrtPath);
            File.Delete(certKeyPath);
            File.Delete(configFilePath);

            return result;
        }

        private static SSL CreateSelfSignedSSLForLinux(string domain)
        {
            var openSSLDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "openssl");

            var baseFileName = Guid.NewGuid().ToString("N");

            //Load base config file, append needed data and save for later usage
            var configFilePath = Path.Combine(openSSLDir, $"{baseFileName}.cnf");
            var configFileContent = File.ReadAllText(Path.Combine(openSSLDir, "openssl_lx.cnf")).Replace("{dir_placeholder}", openSSLDir + "\\ssl") +
                    $"[SAN]\nsubjectAltName=DNS:{domain}";

            File.WriteAllText(configFilePath, configFileContent);

            var args = string.Format("{0} {1} {2} {3}", domain, baseFileName, openSSLDir, configFilePath);

            var psi = new Process
            {
                StartInfo = new ProcessStartInfo($"./{openSSLDir}/openssl-gen.sh", args)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            psi.Start();

            var line = "";
            while (!psi.StandardOutput.EndOfStream)
            {
                line += psi.StandardOutput.ReadLine();
            }

            while (!psi.StandardError.EndOfStream)
            {
                line += psi.StandardError.ReadLine();
            }

            if (!string.IsNullOrEmpty(line))
            {
                line = $"{openSSLDir}/openssl-gen.sh {args}{Environment.NewLine}" + line;
                throw new Exception(line);
            }

            psi.WaitForExit();

            var certCrtPath = Path.Combine(openSSLDir, $"{baseFileName}.crt");
            var certKeyPath = Path.Combine(openSSLDir, $"{baseFileName}.key");

            var result = new SSL()
            {
                CertCrt = File.ReadAllText(certCrtPath),
                CertKey = File.ReadAllText(certKeyPath)
            };

            //Lets clear the path.
            File.Delete(certCrtPath);
            File.Delete(certKeyPath);
            File.Delete(configFilePath);

            return result;
        }

        private static void InstallCertificate(string cerFileName)
        {
#if !DEBUG
            return;
#endif
            var certificate = new X509Certificate2(cerFileName);
            var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

            store.Open(OpenFlags.ReadWrite);
            store.Add(certificate);
            store.Close();
        }
    }
}
