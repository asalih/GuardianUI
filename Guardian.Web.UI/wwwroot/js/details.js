$(document).ready(function () {
    var dataHandlerModel = [
        { chart: "requestChart", header: "REQUEST COUNT (in 30 mins)", type: "request", target: null, labelX: "Time", labelY: "Count" },
        { chart: "ruleChart", header: "PREVENTED MALFORMED REQUESTS (in 30 mins)", type: "rule", target: null, labelX: "Time", labelY: "Count" },
        { chart: "ruleTimeChart", header: "MALFORMED REQUESTS LOG EXECUTION TIME (in 30 mins)", type: "ruleTime", target: null, labelX: "Time", labelY: "Millisecond" }
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

    window.setTimeout(function () {
        for (var i = 0; i < dataHandlerModel.length; i++) {
            initFn(dataHandlerModel[i]);
            window.setInterval(dataHandlerFn, 1000 * 10, dataHandlerModel[i]);
        }
    }, 250);
});