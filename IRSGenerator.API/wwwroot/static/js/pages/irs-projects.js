import { api } from '../api.js';
import { navigate } from '../router.js';

export async function irsProjectsPage() {
  const root = document.getElementById('page-root');
  root.innerHTML = `
    <div class="page-header">
      <div>
        <h1 class="page-title">IRS Projeleri</h1>
        <p class="page-subtitle">Ölçüm kayıt sayfaları</p>
      </div>
      <button class="btn btn-primary" id="btn-new-irs">+ Yeni Proje</button>
    </div>
    <div id="irs-project-grid" class="irs-project-grid">
      <div class="irs-loading">Yükleniyor…</div>
    </div>`;

  document.getElementById('btn-new-irs').addEventListener('click', () => openProjectModal());

  await loadProjects();
}

async function loadProjects() {
  const grid = document.getElementById('irs-project-grid');
  try {
    const [projects, users] = await Promise.all([
      api.irsProjects.list(),
      api.users.list(),
    ]);
    const userMap = {};
    users.forEach(u => { userMap[u.id] = u.name; });

    if (projects.length === 0) {
      grid.innerHTML = `
        <div class="irs-empty-state">
          <div class="irs-empty-icon">📋</div>
          <p class="irs-empty-title">Henüz proje yok</p>
          <p class="irs-empty-sub">Yeni bir IRS projesi oluşturun.</p>
        </div>`;
      return;
    }

    grid.innerHTML = projects.map(p => `
      <div class="irs-project-card" data-id="${p.id}">
        <div class="irs-card-icon">${projectTypeIcon(p.project_type)}</div>
        <div class="irs-card-body">
          <div class="irs-card-type">${p.project_type || '—'}</div>
          <div class="irs-card-pn">${p.part_number}</div>
          <div class="irs-card-meta">
            <span class="irs-meta-item">SN: ${p.serial_number}</span>
            <span class="irs-meta-dot">·</span>
            <span class="irs-meta-item">OP: ${p.operation}</span>
          </div>
        </div>
        <div class="irs-card-footer">
          <span class="irs-card-owner">👤 ${userMap[p.owner_id] || p.owner_id}</span>
          <button class="irs-card-delete btn-icon-danger" data-id="${p.id}" title="Sil">🗑</button>
        </div>
      </div>`).join('');

    grid.querySelectorAll('.irs-project-card').forEach(card => {
      card.addEventListener('click', (e) => {
        if (e.target.closest('.irs-card-delete')) return;
        navigate(`/irs/${card.dataset.id}`);
      });
    });

    grid.querySelectorAll('.irs-card-delete').forEach(btn => {
      btn.addEventListener('click', async (e) => {
        e.stopPropagation();
        if (!confirm('Bu proje silinsin mi? Tüm karakteristikler de silinir.')) return;
        try {
          await api.irsProjects.delete(btn.dataset.id);
          toast('Proje silindi.', 'success');
          loadProjects();
        } catch (err) {
          toast(err.message, 'error');
        }
      });
    });
  } catch (err) {
    grid.innerHTML = `<p class="text-danger">Yüklenemedi: ${err.message}</p>`;
  }
}

function projectTypeIcon(type) {
  const map = { CMM: '🔵', MANUAL: '✋', VISION: '👁', LASER: '🔴' };
  return map[type?.toUpperCase()] || '📐';
}

function openProjectModal(existing = null) {
  modal.open(`
    <div class="modal-header">
      <h3 class="modal-title">${existing ? 'Projeyi Düzenle' : 'Yeni IRS Projesi'}</h3>
    </div>
    <div class="modal-body">
      <div class="form-row-2">
        <div class="form-group">
          <label>Proje Tipi *</label>
          <select id="f-ptype" class="form-input">
            <option value="CMM" ${existing?.project_type === 'CMM' ? 'selected' : ''}>CMM</option>
            <option value="MANUAL" ${existing?.project_type === 'MANUAL' ? 'selected' : ''}>Manuel</option>
            <option value="VISION" ${existing?.project_type === 'VISION' ? 'selected' : ''}>Görüntü</option>
            <option value="LASER" ${existing?.project_type === 'LASER' ? 'selected' : ''}>Lazer</option>
          </select>
        </div>
        <div class="form-group">
          <label>Operasyon No *</label>
          <input type="number" id="f-op" class="form-input" value="${existing?.operation || ''}" placeholder="10" />
        </div>
      </div>
      <div class="form-group">
        <label>Parça Numarası *</label>
        <input type="text" id="f-pn" class="form-input" value="${existing?.part_number || ''}" placeholder="ör. ABC-12345" />
      </div>
      <div class="form-group">
        <label>Seri Numarası *</label>
        <input type="text" id="f-sn" class="form-input" value="${existing?.serial_number || ''}" placeholder="ör. SN-001" />
      </div>
      <div class="form-group">
        <label>Operasyon Sayfası Yolu</label>
        <input type="text" id="f-path" class="form-input" value="${existing?.op_sheet_path || ''}" placeholder="C:\\..." />
      </div>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary" id="m-cancel">İptal</button>
      <button class="btn btn-primary" id="m-save">Kaydet</button>
    </div>`, { wide: false });

  document.getElementById('m-cancel').addEventListener('click', () => modal.close());
  document.getElementById('m-save').addEventListener('click', async () => {
    const data = {
      project_type:  document.getElementById('f-ptype').value,
      operation:     Number(document.getElementById('f-op').value) || 0,
      part_number:   document.getElementById('f-pn').value.trim(),
      serial_number: document.getElementById('f-sn').value.trim(),
      op_sheet_path: document.getElementById('f-path').value.trim(),
      owner_id:      1, // TODO: session user id
    };
    if (!data.part_number || !data.serial_number) {
      toast('Parça ve seri numarası zorunlu.', 'error');
      return;
    }
    try {
      if (existing) {
        await api.irsProjects.update(existing.id, data);
      } else {
        await api.irsProjects.create(data);
      }
      modal.close();
      toast(existing ? 'Güncellendi.' : 'Proje oluşturuldu.', 'success');
      loadProjects();
    } catch (err) {
      toast(err.message, 'error');
    }
  });
}
