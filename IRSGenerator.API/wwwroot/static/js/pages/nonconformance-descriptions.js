import { api } from '../api.js';

export async function nonconformanceDescriptionsPage(id) {
  const root = document.getElementById('page-root');
  root.innerHTML = '<div class="loading">Yükleniyor...</div>';

  let data;
  try {
    data = await api.inspections.ncrDescriptions(id);
  } catch (err) {
    root.innerHTML = `<div class="empty text-danger">Hata: ${err.message}</div>`;
    return;
  }

  const totalNc = data.dimensional.length + data.visual.length;

  const dimRows = data.dimensional.length === 0
    ? '<tr><td colspan="2" class="text-secondary" style="padding:12px;">No dimensional nonconformances.</td></tr>'
    : data.dimensional.map((item, i) => `
        <tr>
          <td style="width:36px;color:var(--text-secondary);font-size:12px;">${i + 1}</td>
          <td class="nc-desc-text">${escHtml(item.description)}</td>
        </tr>`).join('');

  const visRows = data.visual.length === 0
    ? '<tr><td colspan="2" class="text-secondary" style="padding:12px;">No visual nonconformances.</td></tr>'
    : data.visual.map((item, i) => `
        <tr>
          <td style="width:36px;color:var(--text-secondary);font-size:12px;">${i + 1}</td>
          <td class="nc-desc-text">${escHtml(item.description)}</td>
        </tr>`).join('');

  root.innerHTML = `
    <a href="#/inspections/${id}" class="back-link">← Inspection #${id}</a>

    <div class="page-header">
      <h1>Non Conformance Descriptions</h1>
    </div>

    <!-- Inspection header card -->
    <div class="card" style="margin-bottom:16px;">
      <div class="card-header"><span class="card-title">Inspection #${id}</span></div>
      <div style="display:grid;grid-template-columns:repeat(auto-fill,minmax(180px,1fr));gap:8px 24px;padding:12px 16px 16px;font-size:13px;">
        <div><span class="text-secondary">Part No</span><br><strong>${escHtml(data.part_number || '—')}</strong></div>
        <div><span class="text-secondary">Serial No</span><br><strong>${escHtml(data.serial_number || '—')}</strong></div>
        <div><span class="text-secondary">Operation</span><br><strong>${escHtml(data.operation_number || '—')}</strong></div>
        <div><span class="text-secondary">Inspector</span><br><strong>${escHtml(data.inspector || '—')}</strong></div>
        <div><span class="text-secondary">Status</span><br><strong>${escHtml(data.status || '—')}</strong></div>
        <div><span class="text-secondary">Total NC</span><br><strong>${totalNc}</strong></div>
      </div>
    </div>

    <!-- Dimensional -->
    <div class="card" style="margin-bottom:16px;">
      <div class="card-header">
        <span class="card-title">Dimensional Nonconformances (${data.dimensional.length})</span>
      </div>
      <table class="data-table nc-desc-table">
        <tbody>${dimRows}</tbody>
      </table>
    </div>

    <!-- Visual -->
    <div class="card">
      <div class="card-header">
        <span class="card-title">Visual Nonconformances (${data.visual.length})</span>
      </div>
      <table class="data-table nc-desc-table">
        <tbody>${visRows}</tbody>
      </table>
    </div>
  `;
}

function escHtml(str) {
  return String(str ?? '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
}
