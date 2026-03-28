import { api } from '../api.js';

// ── Chart.js UMD yükleyici ─────────────────────────────────────────────────
let _chartJsLoaded = false;
function loadChartJs() {
  return new Promise((resolve, reject) => {
    if (_chartJsLoaded || window.Chart) { _chartJsLoaded = true; resolve(); return; }
    const s = document.createElement('script');
    s.src = '/static/js/vendor/chart.umd.min.js';
    s.onload  = () => { _chartJsLoaded = true; resolve(); };
    s.onerror = () => reject(new Error('Chart.js yüklenemedi'));
    document.head.appendChild(s);
  });
}

// ── Yardımcı fonksiyonlar ──────────────────────────────────────────────────
function formatDateKey(dateStr) {
  try {
    const d = new Date(dateStr);
    return d.toISOString().split('T')[0];
  } catch { return null; }
}

function getLast30Days() {
  const days = [];
  for (let i = 29; i >= 0; i--) {
    const d = new Date();
    d.setDate(d.getDate() - i);
    days.push(d.toISOString().split('T')[0]);
  }
  return days;
}

function getLast7Days() {
  const days = [];
  for (let i = 6; i >= 0; i--) {
    const d = new Date();
    d.setDate(d.getDate() - i);
    days.push(d.toISOString().split('T')[0]);
  }
  return days;
}

function getLast90Days() {
  const days = [];
  for (let i = 89; i >= 0; i--) {
    const d = new Date();
    d.setDate(d.getDate() - i);
    days.push(d.toISOString().split('T')[0]);
  }
  return days;
}

// ── Ana sayfa ──────────────────────────────────────────────────────────────
export async function analyticsPage() {
  const root = document.getElementById('page-root');
  root.innerHTML = '<div class="loading">Analitik yükleniyor...</div>';

  try {
    await loadChartJs();
  } catch (e) {
    root.innerHTML = `<div class="empty text-danger">Chart.js yüklenemedi: ${e.message}</div>`;
    return;
  }

  const [projects, defectTypes] = await Promise.all([
    api.projects.list(),
    api.defectTypes.list(),
  ]);

  const projMap = Object.fromEntries(projects.map(p => [p.id, p.name]));
  const dtMap   = Object.fromEntries(defectTypes.map(d => [d.id, d.name]));

  root.innerHTML = `
    <div class="page-header">
      <h1>Analitik</h1>
    </div>

    <!-- Filtreler -->
    <div class="card" style="padding:16px 20px;">
      <div style="display:flex;gap:16px;flex-wrap:wrap;align-items:flex-end;">
        <div class="form-group" style="margin:0;min-width:180px;">
          <label style="font-size:12px;font-weight:600;color:#6c757d;margin-bottom:4px;display:block;">Proje</label>
          <select class="form-select" id="an-project-filter">
            <option value="">Tüm Projeler</option>
            ${projects.map(p => `<option value="${p.id}">${p.name}</option>`).join('')}
          </select>
        </div>
        <div class="form-group" style="margin:0;min-width:160px;">
          <label style="font-size:12px;font-weight:600;color:#6c757d;margin-bottom:4px;display:block;">Durum</label>
          <select class="form-select" id="an-status-filter">
            <option value="">Tümü</option>
            <option value="open">Açık</option>
            <option value="completed">Tamamlandı</option>
            <option value="rejected">Reddedildi</option>
          </select>
        </div>
        <div class="form-group" style="margin:0;min-width:180px;">
          <label style="font-size:12px;font-weight:600;color:#6c757d;margin-bottom:4px;display:block;">Parça No</label>
          <input type="text" class="form-input" id="an-partno-filter" placeholder="Parça numarası ara…" style="width:100%;" />
        </div>
        <div class="form-group" style="margin:0;min-width:160px;">
          <label style="font-size:12px;font-weight:600;color:#6c757d;margin-bottom:4px;display:block;">Tarih Aralığı</label>
          <select class="form-select" id="an-date-filter">
            <option value="30">Son 30 Gün</option>
            <option value="7">Son 7 Gün</option>
            <option value="90">Son 90 Gün</option>
            <option value="all">Tümü</option>
          </select>
        </div>
        <div id="an-loading" style="display:none;font-size:13px;color:#6c757d;">Yükleniyor...</div>
      </div>
    </div>

    <!-- Özet istatistikler -->
    <div id="an-summary" style="display:grid;grid-template-columns:repeat(auto-fit,minmax(160px,1fr));gap:16px;margin-bottom:0;"></div>

    <!-- Grafikler -->
    <div style="display:grid;grid-template-columns:1fr 1fr;gap:20px;margin-top:20px;" id="an-charts-grid">
      <div class="card" style="padding:20px;">
        <div class="card-title" style="margin-bottom:12px;">Zamana Göre Hata Trendi</div>
        <canvas id="chart-trend" height="220"></canvas>
      </div>
      <div class="card" style="padding:20px;">
        <div class="card-title" style="margin-bottom:12px;">Hata Tipi Pareto</div>
        <canvas id="chart-pareto" height="220"></canvas>
      </div>
      <div class="card" style="padding:20px;">
        <div class="card-title" style="margin-bottom:12px;">Projeye Göre Hata Dağılımı</div>
        <canvas id="chart-project" height="220"></canvas>
      </div>
      <div class="card" style="padding:20px;">
        <div class="card-title" style="margin-bottom:12px;">Muayene Durum Özeti</div>
        <canvas id="chart-status" height="220"></canvas>
      </div>
    </div>
  `;

  const charts = {};

  let _loadDebounce = null;

  function scheduleLoad() {
    clearTimeout(_loadDebounce);
    _loadDebounce = setTimeout(loadData, 300);
  }

  async function loadData() {
    const projectId  = document.getElementById('an-project-filter').value;
    const statusVal  = document.getElementById('an-status-filter').value;
    const partNoVal  = document.getElementById('an-partno-filter').value.trim();
    const dateRange  = document.getElementById('an-date-filter').value;
    const loadingEl  = document.getElementById('an-loading');

    loadingEl.style.display = 'block';

    try {
      // Tüm muayeneleri getir (sunucu tarafı proje + durum filtresi)
      const params = {};
      if (projectId) params.project_id = Number(projectId);
      if (statusVal) params.status = statusVal;
      let inspections = await api.inspections.list(params);

      // Parça no filtresi client-side
      if (partNoVal) {
        inspections = inspections.filter(i =>
          i.part_number && String(i.part_number).toLowerCase().includes(partNoVal.toLowerCase())
        );
      }

      // Her muayene için defect'leri paralel getir
      const defectLists = await Promise.all(
        inspections.map(insp => api.defects.list(insp.id).catch(() => []))
      );

      // Tüm defect'leri düzleştir
      let allDefects = defectLists.flat();

      // Tarih filtresi uygula
      let days = null;
      if (dateRange === '7')  days = new Set(getLast7Days());
      if (dateRange === '30') days = new Set(getLast30Days());
      if (dateRange === '90') days = new Set(getLast90Days());

      if (days) {
        allDefects = allDefects.filter(d => {
          const k = formatDateKey(d.created_at);
          return k && days.has(k);
        });
      }

      // Özet
      renderSummary(inspections, allDefects);

      // Grafik 1: Trend
      const trendDays = dateRange === '7'  ? getLast7Days()
                       : dateRange === '90' ? getLast90Days()
                       : dateRange === 'all' ? buildAllDays(allDefects)
                       : getLast30Days();
      renderTrendChart(allDefects, trendDays, charts);

      // Grafik 2: Pareto
      renderParetoChart(allDefects, dtMap, charts);

      // Grafik 3: Proje
      renderProjectChart(allDefects, inspections, projMap, charts);

      // Grafik 4: Durum
      renderStatusChart(inspections, charts);

    } catch (err) {
      window.toast('Analitik yüklenirken hata: ' + err.message, 'error');
    } finally {
      loadingEl.style.display = 'none';
    }
  }

  // Auto-update on any filter change
  document.getElementById('an-project-filter').addEventListener('change', loadData);
  document.getElementById('an-status-filter').addEventListener('change', loadData);
  document.getElementById('an-date-filter').addEventListener('change', loadData);
  document.getElementById('an-partno-filter').addEventListener('input', scheduleLoad);

  await loadData();
}

function buildAllDays(defects) {
  if (!defects.length) return getLast30Days();
  const keys = defects.map(d => formatDateKey(d.created_at)).filter(Boolean).sort();
  if (!keys.length) return getLast30Days();
  const start = new Date(keys[0]);
  const end   = new Date(keys[keys.length - 1]);
  const days  = [];
  for (let d = new Date(start); d <= end; d.setDate(d.getDate() + 1)) {
    days.push(d.toISOString().split('T')[0]);
  }
  return days.length > 0 ? days : getLast30Days();
}

function renderSummary(inspections, defects) {
  const el = document.getElementById('an-summary');
  const open      = inspections.filter(i => i.status === 'open').length;
  const completed = inspections.filter(i => i.status === 'completed').length;
  const rejected  = inspections.filter(i => i.status === 'rejected').length;

  el.innerHTML = [
    ['Toplam Muayene', inspections.length, '#3b82f6'],
    ['Toplam Hata',    defects.length,     '#ef4444'],
    ['Açık Muayene',   open,               '#f59e0b'],
    ['Tamamlanan',     completed,          '#10b981'],
  ].map(([label, val, color]) => `
    <div class="card" style="padding:16px 20px;border-top:3px solid ${color};">
      <div style="font-size:28px;font-weight:700;color:${color};">${val}</div>
      <div style="font-size:13px;color:#6c757d;margin-top:4px;">${label}</div>
    </div>
  `).join('');
}

function renderTrendChart(defects, days, charts) {
  const countByDay = {};
  for (const d of defects) {
    const k = formatDateKey(d.created_at);
    if (k) countByDay[k] = (countByDay[k] || 0) + 1;
  }

  const labels = days.map(d => {
    const dt = new Date(d + 'T00:00:00');
    return dt.toLocaleDateString('en-US', { day: '2-digit', month: 'short' });
  });
  const data = days.map(d => countByDay[d] || 0);

  const ctx = document.getElementById('chart-trend');
  if (charts.trend) charts.trend.destroy();
  charts.trend = new Chart(ctx, {
    type: 'line',
    data: {
      labels,
      datasets: [{
        label: 'Hata Sayısı',
        data,
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59,130,246,0.1)',
        tension: 0.3,
        fill: true,
        pointRadius: days.length <= 31 ? 4 : 2,
      }],
    },
    options: {
      responsive: true,
      plugins: { legend: { display: false } },
      scales: {
        y: { beginAtZero: true, ticks: { stepSize: 1 } },
        x: { ticks: { maxTicksLimit: 15 } },
      },
    },
  });
}

function renderParetoChart(defects, dtMap, charts) {
  const countByType = {};
  for (const d of defects) {
    const name = dtMap[d.defect_type_id] || `#${d.defect_type_id}`;
    countByType[name] = (countByType[name] || 0) + 1;
  }

  const sorted = Object.entries(countByType).sort((a, b) => b[1] - a[1]);
  const labels  = sorted.map(([name]) => name);
  const counts  = sorted.map(([, c]) => c);
  const total   = counts.reduce((s, c) => s + c, 0);
  let cumulative = 0;
  const cumulativePercent = counts.map(c => {
    cumulative += c;
    return total > 0 ? Math.round((cumulative / total) * 100) : 0;
  });

  const ctx = document.getElementById('chart-pareto');
  if (charts.pareto) charts.pareto.destroy();
  charts.pareto = new Chart(ctx, {
    type: 'bar',
    data: {
      labels,
      datasets: [
        {
          label: 'Hata Sayısı',
          data: counts,
          backgroundColor: 'rgba(239,68,68,0.7)',
          yAxisID: 'y',
        },
        {
          label: 'Kümülatif %',
          data: cumulativePercent,
          type: 'line',
          borderColor: '#f59e0b',
          backgroundColor: 'transparent',
          tension: 0.1,
          pointRadius: 4,
          yAxisID: 'y2',
        },
      ],
    },
    options: {
      responsive: true,
      plugins: { legend: { position: 'top', labels: { font: { size: 11 } } } },
      scales: {
        y:  { beginAtZero: true, ticks: { stepSize: 1 }, position: 'left' },
        y2: { beginAtZero: true, max: 100, position: 'right', grid: { drawOnChartArea: false },
               ticks: { callback: v => v + '%' } },
      },
    },
  });
}

function renderProjectChart(defects, inspections, projMap, charts) {
  // Map inspection → project
  const inspProjMap = Object.fromEntries(inspections.map(i => [i.id, i.project_id]));

  const countByProject = {};
  for (const d of defects) {
    const projId = inspProjMap[d.inspection_id];
    const name   = projId ? (projMap[projId] || `Proje #${projId}`) : 'Proje Yok';
    countByProject[name] = (countByProject[name] || 0) + 1;
  }

  const sorted = Object.entries(countByProject).sort((a, b) => b[1] - a[1]);
  const labels  = sorted.map(([n]) => n);
  const counts  = sorted.map(([, c]) => c);

  const palette = ['#3b82f6','#ef4444','#10b981','#f59e0b','#8b5cf6','#ec4899','#06b6d4','#84cc16'];

  const ctx = document.getElementById('chart-project');
  if (charts.project) charts.project.destroy();
  charts.project = new Chart(ctx, {
    type: labels.length <= 6 ? 'doughnut' : 'bar',
    data: {
      labels,
      datasets: [{
        label: 'Hata Sayısı',
        data: counts,
        backgroundColor: labels.map((_, i) => palette[i % palette.length]),
      }],
    },
    options: {
      responsive: true,
      plugins: { legend: { position: labels.length <= 6 ? 'right' : 'top', labels: { font: { size: 11 } } } },
      ...(labels.length > 6 ? { scales: { y: { beginAtZero: true, ticks: { stepSize: 1 } } } } : {}),
    },
  });
}

function renderStatusChart(inspections, charts) {
  const open      = inspections.filter(i => i.status === 'open').length;
  const completed = inspections.filter(i => i.status === 'completed').length;
  const rejected  = inspections.filter(i => i.status === 'rejected').length;

  const ctx = document.getElementById('chart-status');
  if (charts.status) charts.status.destroy();
  charts.status = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: ['Açık', 'Tamamlandı', 'Reddedildi'],
      datasets: [{
        data: [open, completed, rejected],
        backgroundColor: ['#f59e0b', '#10b981', '#ef4444'],
      }],
    },
    options: {
      responsive: true,
      plugins: { legend: { position: 'right', labels: { font: { size: 12 } } } },
    },
  });
}
