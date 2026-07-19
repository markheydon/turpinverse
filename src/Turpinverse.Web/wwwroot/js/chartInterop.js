window.chartInterop = {
    renderBarChart: function (canvasId, labels, data) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        if (ctx._chart) {
            ctx._chart.destroy();
        }

        ctx._chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Deals',
                    data: data,
                    backgroundColor: '#d97706'
                }]
            },
            options: {
                responsive: true,
                plugins: { legend: { display: false } }
            }
        });
    },

    renderDoughnutChart: function (canvasId, labels, data) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        if (ctx._chart) {
            ctx._chart.destroy();
        }

        ctx._chart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: ['#d97706', '#1e293b', '#f59e0b', '#64748b']
                }]
            },
            options: {
                responsive: true
            }
        });
    }
};
