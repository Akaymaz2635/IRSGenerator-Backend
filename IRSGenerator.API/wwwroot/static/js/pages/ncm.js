import { api }     from '../api.js';
import { session } from '../session.js';

// ── NCM — Inspection List ─────────────────────────────────────────────────
export async function ncmPage() {
  const root = document.getElementById('page-root');
  root.innerHTML = '<div class="loading">Yükleniyor...</div>';

  let inspections = [];
  try {
    inspections = await api.inspections.list();
  } catch (err) {
    root.innerHTML = `<div class="empty text-danger">Hata: ${err.message}</div>`;
    return;
  }

  const isEngineer = session.getRole() === 'engineer';
  const canWrite   = session.getRole() === 'engineer' || session.getRole() === 'admin';

  const rows = inspections.map((ins) => {
    const statusBadge = statusHtml(ins.status);
    return `
      <tr class="ncm-row" data-id="${ins.id}" style="cursor:pointer;">
        <td class="col-id">#${ins.id}</td>
        <td><strong>${escHtml(ins.part_number || '—')}</strong></td>
        <td>${escHtml(ins.serial_number || '—')}</td>
        <td>${escHtml(ins.operation_number || '—')}</td>
        <td>${escHtml(ins.inspector || '—')}</td>
        <td>${statusBadge}</td>
      </tr>`;
  }).join('');

  root.innerHTML = `
    <div class="page-header">
      <h1>Nonconformance Management</h1>
      <span class="text-secondary" style="font-size:13px;">
        ${isEngineer ? 'Engineer View — Disposition sheet oluşturabilirsiniz.' : 'Read-only view.'}
      </span>
    </div>

    <div class="card">
      <div class="card-header">
        <span class="card-title">Inspections (${inspections.length})</span>
        <input id="ncm-search" class="form-control" style="width:220px;"
               placeholder="Part / Serial ara..." />
      </div>

      ${inspections.length === 0
        ? '<div class="empty">Kayıtlı inspection yok.</div>'
        : `<table class="data-table" id="ncm-table">
            <thead>
              <tr>
                <th class="col-id">#</th>
                <th>Part Number</th>
                <th>Serial Number</th>
                <th>Operation</th>
                <th>Inspector</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>${rows}</tbody>
          </table>`
      }
    </div>
  `;

  // Row click → detail
  root.querySelectorAll('.ncm-row').forEach((tr) => {
    tr.addEventListener('click', () => {
      location.hash = `#/ncm/${tr.dataset.id}`;
    });
  });

  // Search filter
  const searchInput = root.querySelector('#ncm-search');
  if (searchInput) {
    searchInput.addEventListener('input', () => {
      const q = searchInput.value.toLowerCase();
      root.querySelectorAll('.ncm-row').forEach((tr) => {
        const text = tr.textContent.toLowerCase();
        tr.style.display = text.includes(q) ? '' : 'none';
      });
    });
  }
}

// ── Helpers ───────────────────────────────────────────────────────────────
function statusHtml(status) {
  const map = {
    open:      ['badge-info',    'Open'],
    in_progress: ['badge-warning', 'In Progress'],
    completed: ['badge-success', 'Completed'],
    closed:    ['badge-neutral', 'Closed'],
  };
  const [cls, label] = map[status] ?? ['badge-neutral', status];
  return `<span class="badge ${cls}">${label}</span>`;
}

function escHtml(str) {
  return String(str ?? '')
    .replace(/&/g, '&amp;').replace(/</g, '&lt;')
    .replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}
