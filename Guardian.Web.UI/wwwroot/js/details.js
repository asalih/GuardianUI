$(document).ready(function () {
    var dataHandlerModel = [
        { chart: "requestChart", type: "request", handler: function () { } },
        { chart: "ruleChart", type: "rule", handler: function () { } }
    ];

    var handlerFn = function (data) {
        $.getJSON("/targets/report/" + $("#id").val() + "?type=" + data.type, function (report) {

            if (report.result.length == 0) {
                return;
            }

            var labels = [];
            var dataSet = [];

            for (var i = 0; i < report.result.length; i++) {
                labels.push(report.result[i].dateTime);
                dataSet.push(report.result[i].value);
            }

            var chart = new Chart(document.getElementById(data.chart),
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
                        responsive: true,
                        title: {
                            display: true,
                            text: 'Chart.js Line Chart'
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
                                    labelString: 'Month'
                                }
                            }],
                            yAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Value'
                                }
                            }]
                        }
                    }
                });
        });
    };

    window.setTimeout(function () {
        for (var i = 0; i < dataHandlerModel.length; i++) {
            window.setInterval(handlerFn, 5000, dataHandlerModel[i]);
        }
    }, 250);
});