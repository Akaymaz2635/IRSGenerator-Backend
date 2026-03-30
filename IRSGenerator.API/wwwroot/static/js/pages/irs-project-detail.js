import { api } from '../api.js';
import { navigate } from '../router.js';

const RESULT_META = {
  WithinTolerance:          { label: 'Tolerans İçi',       cls: 'result-ok'      },
  OutOfTolerance:           { label: 'Tolerans Dışı',      cls: 'result-fail'    },
  WrongFormat:              { label: 'Yanlış Format',       cls: 'result-warn'    },
  MinMaxValueOverTolerance: { label: 'Min/Max Fazla',       cls: 'result-over'    },
  MinMaxValueUnderTolerance:{ label: 'Min/Max Az',          cls: 'result-under'   },
  MaxValueOverTolerance:    { label: 'Max Fazla',           cls: 'result-over'    },
  MinValueUnderTolerance:   { label: 'Min Az',              cls: 'result-under'   },
  Unidentified:             { label: 'Belirsiz',            cls: 'result-none'    },
};

export async function irsProjectDetailPage(projectId) {
  const root = document.getElementById('page-root');
  root.innerHTML = `<div class="irs-loading">Yükleniyor…</div>`;

  try {
    const project = await api.irsProjects.get(projectId);
    const chars   = await api.characters.list({ irs_project_id: projectId });

    const withinCount = chars.filter(c => c.inspection_result === 'WithinTolerance').length;
    const failCount   = chars.filter(c => c.inspection_result === 'OutOfTolerance').length;

    root.innerHTML = `
      <div class="page-header">
        <div>
          <div class="breadcrumb">
            <a href="#/irs" class="breadcrumb-link">IRS Projeleri</a>
            <span class="breadcrumb-sep">›</span>
            <span>${project.part_number} / ${project.serial_number}</span>
          </div>
          <h1 class="page-title">${project.part_number}</h1>
          <p class="page-subtitle">${project.project_type} · Operasyon ${project.operation} · SN: ${project.serial_number}</p>
        </div>
        <button class="btn btn-primary" id="btn-add-char">+ Karakteristik Ekle</button>
      </div>

      <div class="irs-stats-row">
        <div class="irs-stat-card">
          <div class="irs-stat-value">${chars.length}</div>
          <div class="irs-stat-label">Toplam</div>
        </div>
        <div class="irs-stat-card irs-stat-ok">
          <div class="irs-stat-value">${withinCount}</div>
          <div class="irs-stat-label">Tolerans İçi</div>
        </div>
        <div class="irs-stat-card irs-stat-fail">
          <div class="irs-stat-value">${failCount}</div>
          <div class="irs-stat-label">Tolerans Dışı</div>
        </div>
        <div class="irs-stat-card">
          <div class="irs-stat-value">${chars.length - withinCount - failCount}</div>
          <div class="irs-stat-label">Değerlendirilmedi</div>
        </div>
      </div>

      <div class="irs-table-card">
        <table class="irs-char-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Item No</th>
              <th>Tolerans</th>
              <th>Alt Sınır</th>
              <th>Üst Sınır</th>
              <th>Sonuç</th>
              <th></th>
            </tr>
          </thead>
          <tbody id="char-tbody">
            ${chars.length === 0
              ? `<tr><td colspan="7" class="irs-empty-row">Henüz karakteristik yok. "+ Karakteristik Ekle" ile başlayın.</td></tr>`
              : chars.map((c, i) => charRow(c, i + 1)).join('')}
          </tbody>
        </table>
      </div>`;

    document.getElementById('btn-add-char').addEventListener('click', () =>
      openCharModal(projectId, null, () => irsProjectDetailPage(projectId)));

    document.getElementById('char-tbody').querySelectorAll('[data-char-id]').forEach(row => {
      row.addEventListener('click', (e) => {
        if (e.target.closest('.char-row-del')) return;
        navigate(`/irs/${projectId}/characters/${row.dataset.charId}`);
      });
    });

    document.getElementById('char-tbody').querySelectorAll('.char-row-del').forEach(btn => {
      btn.addEventListener('click', async (e) => {
        e.stopPropagation();
        if (!confirm('Bu karakteristik silinsin mi?')) return;
        try {
          await api.characters.delete(btn.dataset.id);
          toast('Karakteristik silindi.', 'success');
          irsProjectDetailPage(projectId);
        } catch (err) {
          toast(err.message, 'error');
        }
      });
    });
  } catch (err) {
    root.innerHTML = `<p class="text-danger">Yüklenemedi: ${err.message}</p>`;
  }
}

function charRow(c, index) {
  const meta = RESULT_META[c.inspection_result] || RESULT_META.Unidentified;
  const hasLimits = c.lower_limit !== 0 || c.upper_limit !== 0;
  return `
    <tr class="char-row ${meta.cls}-row" data-char-id="${c.id}">
      <td class="char-index">${index}</td>
      <td class="char-item-no">${c.item_no}</td>
      <td class="char-dim">${c.dimension}</td>
      <td class="char-limit">${hasLimits ? c.lower_limit.toFixed(4) : '—'}</td>
      <td class="char-limit">${hasLimits ? c.upper_limit.toFixed(4) : '—'}</td>
      <td><span class="result-badge ${meta.cls}">${meta.label}</span></td>
      <td class="char-actions">
        <button class="char-row-del btn-icon-danger" data-id="${c.id}" title="Sil">🗑</button>
      </td>
    </tr>`;
}

export function openCharModal(projectId, existing, onSaved) {
  modal.open(`
    <div class="modal-header">
      <h3 class="modal-title">${existing ? 'Karakteristik Düzenle' : 'Yeni Karakteristik'}</h3>
    </div>
    <div class="modal-body">
      <div class="form-row-2">
        <div class="form-group">
          <label>Item No *</label>
          <input type="text" id="fc-itemno" class="form-input" value="${existing?.item_no || ''}" placeholder="ör. 1, 1.1, A2" />
        </div>
        <div class="form-group">
          <label>Tolerans / Dimension *</label>
          <input type="text" id="fc-dim" class="form-input" value="${existing?.dimension || ''}" placeholder="ör. 10±0.5 / ⌀0.2 / MAX 5" />
        </div>
      </div>
      <div class="form-row-2">
        <div class="form-group">
          <label>Badge</label>
          <input type="text" id="fc-badge" class="form-input" value="${existing?.badge || ''}" placeholder="ör. ⌀" />
        </div>
        <div class="form-group">
          <label>BP Zone</label>
          <input type="text" id="fc-zone" class="form-input" value="${existing?.bp_zone || ''}" placeholder="ör. A, B1" />
        </div>
      </div>
      <div class="form-row-2">
        <div class="form-group">
          <label>Tooling</label>
          <input type="text" id="fc-tooling" class="form-input" value="${existing?.tooling || ''}" />
        </div>
        <div class="form-group">
          <label>Inspection Level</label>
          <input type="text" id="fc-level" class="form-input" value="${existing?.inspection_level || ''}" placeholder="ör. I, II, III" />
        </div>
      </div>
      <div class="form-group">
        <label>Notlar</label>
        <input type="text" id="fc-remarks" class="form-input" value="${existing?.remarks || ''}" />
      </div>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary" id="mc-cancel">İptal</button>
      <button class="btn btn-primary" id="mc-save">Kaydet</button>
    </div>`);

  document.getElementById('mc-cancel').addEventListener('click', () => modal.close());
  document.getElementById('mc-save').addEventListener('click', async () => {
    const data = {
      item_no:          document.getElementById('fc-itemno').value.trim(),
      dimension:        document.getElementById('fc-dim').value.trim(),
      badge:            document.getElementById('fc-badge').value.trim() || null,
      bp_zone:          document.getElementById('fc-zone').value.trim() || null,
      tooling:          document.getElementById('fc-tooling').value.trim() || null,
      inspection_level: document.getElementById('fc-level').value.trim() || null,
      remarks:          document.getElementById('fc-remarks').value.trim() || null,
      irs_project_id:   projectId,
    };
    if (!data.item_no || !data.dimension) {
      toast('Item No ve Tolerans zorunlu.', 'error'); return;
    }
    try {
      if (existing) {
        await api.characters.update(existing.id, data);
      } else {
        await api.characters.create(data);
      }
      modal.close();
      toast(existing ? 'Güncellendi.' : 'Karakteristik eklendi.', 'success');
      onSaved?.();
    } catch (err) { toast(err.message, 'error'); }
  });
}
