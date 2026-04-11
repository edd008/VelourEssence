// dashboard-echarts.js
document.addEventListener('DOMContentLoaded', function () {
    // Paleta
    const colors = ["#1F3A5F", "#2C5282", "#A8E6CF", "#FF8C94"];

    // Tooltip pro
    const tooltipStyle = {
        backgroundColor: "rgba(255,255,255,0.95)",
        borderColor: "#E2E8F0",
        borderWidth: 1,
        textStyle: {
            color: "#2D3748",
            fontSize: 13
        },
        extraCssText: "border-radius: 8px; padding: 8px 12px; box-shadow: 0 4px 14px rgba(0,0,0,0.08);"
    };

    // --- Ventas por Día (Line Chart con gradiente) ---
    var salesByDayChart = echarts.init(document.getElementById('salesByDayChart'));
    var salesByDayOption = {
        tooltip: {
            trigger: 'axis',
            ...tooltipStyle
        },
        grid: { left: 40, right: 20, top: 40, bottom: 40 },
        xAxis: {
            type: 'category',
            data: window.salesByDayLabels,
            axisLine: { lineStyle: { color: "#CBD5E0" } },
            axisLabel: { color: "#4A5568" }
        },
        yAxis: {
            type: 'value',
            axisLine: { show: false },
            splitLine: { lineStyle: { color: "#EDF2F7" } },
            axisLabel: { color: "#4A5568" }
        },
        series: [{
            data: window.salesByDayData,
            type: 'line',
            smooth: true,
            showSymbol: true,
            symbolSize: 8,
            lineStyle: { width: 3, color: colors[0] },
            areaStyle: {
                color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                    { offset: 0, color: "rgba(31,58,95,0.35)" },
                    { offset: 1, color: "rgba(31,58,95,0.05)" }
                ])
            }
        }]
    };
    salesByDayChart.setOption(salesByDayOption);

    // --- Pedidos por Estado (Pie Chart moderno) ---
    var ordersByStatusChart = echarts.init(document.getElementById('ordersByStatusChart'));
    var ordersByStatusOption = {
        tooltip: {
            trigger: 'item',
            ...tooltipStyle,
            formatter: '{b}: {c} ({d}%)'
        },
        legend: {
            bottom: 0,
            textStyle: { color: "#4A5568" },
            itemWidth: 16,
            itemHeight: 16
        },
        series: [{
            name: 'Pedidos',
            type: 'pie',
            radius: ['40%', '70%'],
            avoidLabelOverlap: true,
            itemStyle: {
                borderRadius: 10,
                borderColor: '#fff',
                borderWidth: 2
            },
            label: {
                show: false,
                position: 'center'
            },
            emphasis: {
                label: {
                    show: true,
                    fontSize: 16,
                    fontWeight: 'bold',
                    color: colors[1]
                }
            },
            labelLine: { show: false },
            data: window.ordersByStatusData
        }]
    };
    ordersByStatusChart.setOption(ordersByStatusOption);

    // --- Ventas por Mes (Bar Chart con gradiente y bordes redondeados) ---
    var salesByMonthChart = echarts.init(document.getElementById('salesByMonthChart'));
    var salesByMonthOption = {
        tooltip: {
            trigger: 'axis',
            ...tooltipStyle
        },
        grid: { left: 40, right: 20, top: 40, bottom: 40 },
        xAxis: {
            type: 'category',
            data: window.salesByMonthLabels,
            axisLine: { lineStyle: { color: "#CBD5E0" } },
            axisLabel: { color: "#4A5568" }
        },
        yAxis: {
            type: 'value',
            axisLine: { show: false },
            splitLine: { lineStyle: { color: "#EDF2F7" } },
            axisLabel: { color: "#4A5568" }
        },
        series: [{
            data: window.salesByMonthData,
            type: 'bar',
            barWidth: '50%',
            itemStyle: {
                borderRadius: [6, 6, 0, 0],
                color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                    { offset: 0, color: colors[1] },
                    { offset: 1, color: "rgba(44,82,130,0.4)" }
                ])
            }
        }]
    };
    salesByMonthChart.setOption(salesByMonthOption);

    // --- Responsividad ---
    window.addEventListener('resize', () => {
        salesByDayChart.resize();
        ordersByStatusChart.resize();
        salesByMonthChart.resize();
    });
});
