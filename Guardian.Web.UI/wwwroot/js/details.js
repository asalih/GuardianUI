$(document).ready(function () {
    var chartDataHandlerModel = [
        { chart: "requestChart", header: "REQUEST COUNT (in 30 mins)", type: "request", target: null, labelX: "Time", labelY: "Count", interval: 10000 },
        { chart: "requestTimeChart", header: "ORIGIN HTTP REQUEST TIME (in 30 mins)", type: "requestTime", target: null, labelX: "Time", labelY: "Millisecond", interval: 11000 },
        { chart: "ruleChart", header: "UNSECURE REQUESTS (in 30 mins)", type: "rule", target: null, labelX: "Time", labelY: "Count", interval: 12000 },
        { chart: "ruleTimeChart", header: "AVERAGE RULE EXECUTION TIME (in 30 mins)", type: "ruleTime", target: null, labelX: "Time", labelY: "Millisecond", interval: 13000 }
    ];

    var summaryDataHandlerModel = [
        { chart: "requestRuleRatio", header: "REQUEST RULE RATIO", type: "requestRuleRatio", target: null, interval: 14000 }
    ];

    var initFn = function (data) {
        $.getJSON("/targets/report/" + $("#id").val() + "?type=" + data.type, function (report) {

            if (report.result.length == 0) {
                return;
            }

            var labels = [];
            var dataSet = [];

            for (var i = 0; i < report.result.length; i++) {
                labels.push(report.result[i].time);
                dataSet.push(report.result[i].value);
            }

            data.target = new Chart(document.getElementById(data.chart),
                {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: [
                            {
                                data: dataSet,
                                backgroundColor: 'rgba(255, 99, 132, 0.1)',
                                borderColor: 'rgb(255, 99, 132)',
                                borderWidth: 2,
                                lineTension: 0.25,
                                pointRadius: 0
                            }
                        ]
                    },
                    options: {
                        legend: {
                            display: false
                        },
                        responsive: true,
                        title: {
                            display: true,
                            text: data.header
                        },
                        tooltips: {
                            mode: 'index',
                            intersect: false,
                        },
                        hover: {
                            mode: 'nearest',
                            intersect: true
                        },
                        scales: {
                            xAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: data.labelX
                                }
                            }],
                            yAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: data.labelY
                                }
                            }]
                        }
                    }
                });
        });
    };

    var dataHandlerFn = function (data) {
        $.getJSON("/targets/report/" + $("#id").val() + "?type=" + data.type, function (report) {
            if (report.result.length == 0) {
                return;
            }

            var labels = data.target.data.labels = [];
            var dataSet = data.target.data.datasets[0].data = [];

            for (var i = 0; i < report.result.length; i++) {
                labels.push(report.result[i].time);
                dataSet.push(report.result[i].value);
            }

            data.target.labels = labels;
            data.target.data.datasets[0].data = dataSet;
            data.target.update();
        });
    };

    var summaryDataHandler = function (data) {
        $.getJSON("/targets/report/" + $("#id").val() + "?type=" + data.type, function (report) {
            var $chart = $("#"+data.chart);

            if (report.result.length == 0) {
                $chart.text(0);
                return;
            }

            $chart.text(report.result[0].value);
        });
    };

    window.setTimeout(function () {
        for (var i = 0; i < chartDataHandlerModel.length; i++) {
            var dModel = chartDataHandlerModel[i];
            initFn(dModel);
            window.setInterval(dataHandlerFn, dModel.interval, dModel);
        }

        for (var j = 0; j < summaryDataHandlerModel.length; j++) {
            var summaryDModel = summaryDataHandlerModel[j];
            summaryDataHandler(summaryDModel);
            window.setInterval(summaryDataHandler, summaryDModel.interval, summaryDModel);
        }
    }, 200);
});