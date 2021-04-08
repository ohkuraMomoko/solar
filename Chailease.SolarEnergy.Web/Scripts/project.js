$(function() {
	//slider ================================
	$(".js-project-slider").slick({
		dots: true,
		autoplay: true,
		arrows: true,
		autoplaySpeed: 5000
	});

    
	//youtube ===================================
	var youtubeUrlPrefix = "https://www.youtube.com/embed/";
	var shareUrlPrefix = "https://youtu.be/";
	//問號會讓match/seatch/includes都判斷錯誤，加上\\跳脫
	var watchUrl = "https://www.youtube.com/watch\\?v=";
	var embed = "https://www.youtube.com/embed/";

	function getYoutubeId(url) {
		var res;
		if (url.match(shareUrlPrefix)) {
			res = url.replace(shareUrlPrefix, youtubeUrlPrefix);
		} else if (url.match(watchUrl)) {
			res = youtubeUrlPrefix + url.split("v=")[1];
		} else if (url.match(embed)) {
			res = url;
        } else {
            showMsg("error","error");
			return;
		}
		return res;
	}

	$("#modalVideo").on("show.bs.modal", function(e) {
		var youtubeid = $(e.relatedTarget).data("youtube");
		youtubeid = getYoutubeId(youtubeid);
		console.log(youtubeid);
		$("#modalVideoIndex").append(
			"<div id='moviePlayer'><iframe src='" +
				youtubeid +
				"?rel=0' frameborder='0' allow='autoplay; encrypted-media' autoplay='1' allowfullscreen'></iframe></div>"
		);
	});

	$("#modalVideo").on("hidden.bs.modal", function(e) {
		$("#moviePlayer").remove();
	});

});

//window.setChart = function (modelData) {

//    // 試算圖表 =========================================
//    const primaryColor = "rgba(20, 112, 107, 1)";
//    const primaryColorOpacity = "rgba(20, 112, 107, .85)";
//    const calcChart = $("#calcChart");
//    var chart = document.getElementById("calcChart").getContext("2d"),
//        gradient = chart.createLinearGradient(0, 0, 0, 450);
//    gradient.addColorStop(0, "rgba(34, 218, 212, .8)");
//    gradient.addColorStop(0.3, "rgba(34, 218, 212, .32)");
//    gradient.addColorStop(1, "rgba(34, 218, 212, 0.1)");

//    var customTooltips = function (tooltip) {
//        // Tooltip Element
//        var tooltipEl = document.getElementById("chartjs-tooltip");
//        if (!tooltipEl) {
//            tooltipEl = document.createElement("div");
//            tooltipEl.id = "chartjs-tooltip";
//            tooltipEl.innerHTML = "<table></table>";
//            this._chart.canvas.parentNode.appendChild(tooltipEl);
//        }

//        // Hide if no tooltip
//        if (tooltip.opacity === 0) {
//            tooltipEl.style.opacity = 0;
//            return;
//        }

//        // Set caret Position
//        tooltipEl.classList.remove("above", "below", "no-transform");
//        if (tooltip.yAlign) {
//            tooltipEl.classList.add(tooltip.yAlign);
//        } else {
//            tooltipEl.classList.add("no-transform");
//        }

//        function getBody(bodyItem) {
//            return bodyItem.lines;
//        }

//        // Set Text
//        if (tooltip.body) {
//            var titleLines = tooltip.title || [];
//            var bodyLines = tooltip.body.map(getBody);
//            innerHtml = "<tbody>";

//            bodyLines.forEach(function (body, i) {
//                // let value = body.toString().replace(/[^0-9]/g, "");
//                //去除冒號
//                let value = body.toString().replace(/\:/g, '');
//                // TODO: 金額要加$字號與負號 參閱UI SPEC
//                // TODO: 屋頂租金/保險費用/其他費用 會是負號
//                innerHtml +=
//                    "<tr><td>預估電費 :</td><td>" +
//                    "$" + setSymbol(value) +
//                    "</td></tr>" +
//                    "<tr><td>屋頂租金 :</td><td>" +
//                    "$" + setSymbol(value) +
//                    "</td></tr>" +
//                    "<tr><td>保險費用 :</td><td>" +
//                    "$" + setSymbol(value) +
//                    "</td></tr>" +
//                    "<tr><td>其他費用 :</td><td>" +
//                    "$" + setSymbol(value) +
//                    "</td></tr>" +
//                    "<tr><td>收款金額 :</td><td style='color: #FFF3B5;'>" +
//                    "$" + setSymbol(value) +
//                    "</td></tr>" +
//                    "<tr><td>累積收款金額 :</td><td style='color: #FFF3B5;'>" +
//                    "$" + setSymbol(value) +
//                    "</td></tr>";
//            });
//            innerHtml += "</tbody>";
//            var tableRoot = tooltipEl.querySelector("table");
//            tableRoot.innerHTML = innerHtml;
//        }
//        var positionY = this._chart.canvas.offsetTop;
//        var positionX = this._chart.canvas.offsetLeft;

//        // Display, position, and set styles for font
//        tooltipEl.style.opacity = 1;
//        if (tooltip.caretX >= calcChart.width() * 0.6) {
//            tooltipEl.style.right = calcChart.width() - tooltip.caretX + "px";
//            tooltipEl.style.left = "unset";
//        } else {
//            tooltipEl.style.left = positionX + tooltip.caretX + "px";
//            tooltipEl.style.right = "unset";
//        }

//        tooltipEl.style.top = positionY + tooltip.caretY + 10 + "px";
//        // tooltipEl.style.fontFamily = tooltip._bodyFontFamily;
//        // tooltipEl.style.fontSize = tooltip.bodyFontSize + 'px';
//        // tooltipEl.style.fontStyle = tooltip._bodyFontStyle;
//        // tooltipEl.style.padding = tooltip.yPadding + 'px ' + tooltip.xPadding + 'px';
//    };

//    let year = 20;
//    let chartData = modelData

//    function randomData(year) {
//        let data = [];
//        for (let i = 0; i <= year; i++) {
//            data.push(randomScalingFactor() * 100);
//        }
//        return data;
//    }


//    var config = {
//        type: "line",
//        data: {
//            // X軸 20年收益
//            labels: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20],
//            datasets: [
//                {
//                    // "label": "收款金額",
//                    data: chartData,
//                    // "fill": false,
//                    borderWidth: 2,
//                    borderColor: "rgb(145, 159, 158)",
//                    // 曲線0.4~直線0
//                    lineTension: 0,
//                    // 漸層背景
//                    backgroundColor: gradient,
//                    pointBackgroundColor: primaryColor,
//                    pointBorderColor: primaryColor,
//                    pointBorderWidth: 3,
//                    pointHoverBorderWidth: 4,
//                    // borser style: dash
//                    borderDash: [5, 5]
//                }
//            ]
//        },
//        options: {
//            responsive: true,
//            //圖表比例用樣式控制(.chart-container)
//            maintainAspectRatio: false,
//            legend: {
//                display: false
//            },
//            tooltips: {
//                enabled: false,
//                position: "nearest",
//                mode: "index",
//                intersect: false,
//                backgroundColor: primaryColorOpacity,
//                displayColors: false,
//                custom: customTooltips
//            },
//            scales: {
//                yAxes: [
//                    {
//                        ticks: {
//                            autoSkip: false,
//                            // 金額區段
//                            fixedStepSize: 5000,
//                            callback: function (value, index, values) {
//                                return setSymbol(value);
//                            }
//                        },
//                        // scaleLabel: {
//                        //     display: false,
//                        // },
//                        stacked: false,
//                        gridLines: {
//                            display: true,
//                            color: "rgba(0,0,0,0.2)"
//                        }
//                    }
//                ],
//                xAxes: [
//                    {
//                        ticks: {
//                            autoSkip: true,
//                            autoSkipPadding: 10,
//                            maxRotation: false
//                        },
//                        scaleLabel: {
//                            display: true
//                        },
//                        gridLines: {
//                            display: true,
//                            color: "rgba(0,0,0,0.2)"
//                        }
//                    }
//                ]
//            },
//            annotation: {
//                events: ["click"],
//                annotations: [
//                    {
//                        drawTime: "afterDatasetsDraw",
//                        id: "hline",
//                        type: "line",
//                        mode: "horizontal",
//                        scaleID: "y-axis-0",
//                        // TODO: 基準線為購入成本??
//                        value: 17811,
//                        borderColor: "rgb(255, 243, 181)",
//                        borderWidth: 3
//                        // label: {
//                        //     backgroundColor: "red",
//                        //     content: "購入成本",
//                        //     enabled: true
//                        // },
//                        // onClick: function (e) {
//                        //     console.log("Annotation", e.type, this);
//                        // }
//                    }
//                ]
//            }
//        }
//    };

//    window.myLine = new Chart(document.getElementById("calcChart"), config);

//    function setSymbol(num) {
//        // return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
//        var source = String(num).split(".");
//        source[0] = source[0].replace(new RegExp('(\\d)(?=(\\d{3})+$)', 'ig'), "$1,");
//        return source.join(".");
//    }

//    var data = { tempChartData: chartData };
//    $(".js-tableData").children(".item-box").remove();
//    $("#template").tmpl(data).appendTo(".js-tableData");
//    $("#templateTotal").tmpl(data).appendTo(".js-tableData");

//    //測試塞隨機資料 ----
//    document.getElementById("getCalc").addEventListener("click", function () {
//        config.data.datasets.forEach(function (dataset) {
//            dataset.data = dataset.data.map(function () {
//                return randomScalingFactor() * 100;
//            });
//            chartData = dataset.data;
//        });

//        window.myLine.update();
//        data = { tempChartData: chartData };
//        $(".js-tableData").children(".item-box").remove();
//        $("#template").tmpl(data).appendTo(".js-tableData");
//        $("#templateTotal").tmpl(data).appendTo(".js-tableData");
//    });
//}
