import { api } from '../api.js';
import { session } from '../session.js';

function statusBadge(status) {
  const map    = { open: 'badge-open', completed: 'badge-completed', rejected: 'badge-rejected' };
  const labels = { open: 'Open', completed: 'Completed', rejected: 'Rejected' };
  return `<span class="badge ${map[status] || ''}">${labels[status] || status}</span>`;
}

function formatDate(str) {
  if (!str) return '—';
  try {
    return new Date(str).toLocaleDateString('en-US', { day: '2-digit', month: 'short', year: 'numeric' });
  } catch { return str; }
}

let _debounceTimer = null;

export async function inspectionsPage() {
  const root = document.getElementById('page-root');
  root.innerHTML = '<div class="loading">Yükleniyor...</div>';

  const projects = await api.projects.list();
  const projMap = {};
  for (const p of projects) projMap[p.id] = p.name;

  const projectOptions = projects.map((p) =>
    `<option value="${p.id}">${p.name}</option>`
  ).join('');

  root.innerHTML = `
    <div class="page-header">
      <h1>Inspections</h1>
      <div class="page-header-actions">
        ${session.canWrite() ? '<a href="#/inspections/new" class="btn btn-primary">+ New Inspection</a>' : ''}
      </div>
    </div>

    <div class="filter-bar">
      <input type="text" class="form-input" id="search-input" placeholder="Search part no, serial no or inspector..." />
      <select class="form-select" id="status-filter">
        <option value="">All Statuses</option>
        <option value="open">Open</option>
        <option value="completed">Completed</option>
        <option value="rejected">Rejected</option>
      </select>
      <select class="form-select" id="project-filter">
        <option value="">Tüm Projeler</option>
        ${projectOptions}
      </select>
    </div>

    <div class="card" style="padding:0;overflow:hidden;">
      <div id="insp-table-area">
        <div class="loading">Yükleniyor...</div>
      </div>
    </div>
  `;

  const searchInput   = root.querySelector('#search-input');
  const statusFilter  = root.querySelector('#status-filter');
  const projectFilter = root.querySelector('#project-filter');
  const tableArea     = root.querySelector('#insp-table-area');

  async function loadTable() {
    const params = {
      search:     searchInput.value.trim() || undefined,
      status:     statusFilter.value || undefined,
      project_id: projectFilter.value ? Number(projectFilter.value) : undefined,
    };

    try {
      const inspections = await api.inspections.list(params);
      if (inspections.length === 0) {
        tableArea.innerHTML = '<div class="empty">No inspections found.</div>';
        return;
      }

      const rows = inspections.map((insp) => `
        <tr>
          <td class="col-id">#${insp.id}</td>
          <td>${insp.part_number || '—'}</td>
          <td>${insp.serial_number || '—'}</td>
          <td>${insp.operation_number || '—'}</td>
          <td>${projMap[insp.project_id] || '—'}</td>
          <td>${insp.inspector || '—'}</td>
          <td>${statusBadge(insp.status)}</td>
          <td class="text-secondary">${formatDate(insp.created_at)}</td>
          <td class="col-actions">
            <a href="#/inspections/${insp.id}" class="btn btn-ghost btn-xs">Aç</a>
            ${session.canWrite() ? `<a href="#/inspections/${insp.id}/edit" class="btn btn-ghost btn-xs">Düzenle</a>` : ''}
            ${session.canWrite() ? `<button class="btn btn-danger btn-xs delete-btn" data-id="${insp.id}">Sil</button>` : ''}
          </td>
        </tr>
      `).join('');

      tableArea.innerHTML = `
        <table class="data-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Parça No</th>
              <th>Seri No</th>
              <th>Op. No</th>
              <th>Proje</th>
              <th>Inspector</th>
              <th>Status</th>
              <th>Date</th>
              <th></th>
            </tr>
          </thead>
          <tbody>${rows}</tbody>
        </table>
      `;

      tableArea.querySelectorAll('.delete-btn').forEach((btn) => {
        btn.addEventListener('click', async () => {
          const id = Number(btn.dataset.id);
          if (!confirm(`Are you sure you want to delete inspection #${id}?`)) return;
          try {
            await api.inspections.delete(id);
            window.toast('Inspection deleted.', 'success');
            await loadTable();
          } catch (err) {
            window.toast('Silme hatası: ' + err.message, 'error');
          }
        });
      });
    } catch (err) {
      tableArea.innerHTML = `<div class="empty text-danger">Hata: ${err.message}</div>`;
    }
  }

  searchInput.addEventListener('input', () => {
    clearTimeout(_debounceTimer);
    _debounceTimer = setTimeout(loadTable, 400);
  });
  statusFilter.addEventListener('change', loadTable);
  projectFilter.addEventListener('change', loadTable);

  await loadTable();
}
