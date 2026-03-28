import { api } from '../api.js';

export async function adminDispositionTypesPage(container) {
  await renderPage(container);
}

async function renderPage(container) {
  let types, transitions;
  try {
    [types, transitions] = await Promise.all([
      api.dispositionTypes.list(true),
      api.dispositionTransitions.list(),
    ]);
  } catch (e) {
    container.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Yüklenemedi: ${e.message}</div>`;
    return;
  }

  // Build transition map: from_code|'__null__' -> Set<to_code>
  const transMap = {};
  for (const t of transitions) {
    const key = t.from_code ?? '__null__';
    if (!transMap[key]) transMap[key] = new Set();
    transMap[key].add(t.to_code);
  }

  container.innerHTML = `
    <div class="admin-page-header">
      <div>
        <h1>Karar Tipleri</h1>
        <p>${types.length} kayıt</p>
      </div>
      <button class="btn btn-primary" id="btn-add">+ Yeni Karar Tipi</button>
    </div>

    <div class="admin-card">
      <div class="admin-table-wrap">
        <table>
          <thead>
            <tr>
              <th>Kod</th>
              <th>Etiket / Görünüm</th>
              <th>Defekti Kapatır</th>
              <th>Başlangıç</th>
              <th>Sıra</th>
              <th>Durum</th>
              <th>İşlemler</th>
            </tr>
          </thead>
          <tbody>
            ${types.length === 0
              ? '<tr><td colspan="7" style="text-align:center;color:var(--text-muted);padding:32px;">Karar tipi bulunamadı</td></tr>'
              : types.map(t => `
                <tr>
                  <td><code style="font-size:11px;">${esc(t.code)}</code></td>
                  <td><span class="disp-badge ${esc(t.css_class)}">${esc(t.label)}</span></td>
                  <td style="text-align:center;">${t.is_neutralizing ? '✅' : '—'}</td>
                  <td style="text-align:center;">${t.is_initial ? '✅' : '—'}</td>
                  <td style="text-align:center;">${t.sort_order}</td>
                  <td>
                    <span class="badge ${t.active ? 'badge-active' : 'badge-inactive'}">
                      ${t.active ? 'Aktif' : 'Pasif'}
                    </span>
                  </td>
                  <td>
                    <div class="tbl-actions">
                      <button class="btn btn-sm btn-ghost btn-edit-type" data-id="${t.id}">Düzenle</button>
                      ${t.active
                        ? `<button class="btn btn-sm btn-danger btn-toggle-type" data-id="${t.id}" data-active="1">Pasife Al</button>`
                        : `<button class="btn btn-sm btn-secondary btn-toggle-type" data-id="${t.id}" data-active="0">Aktife Al</button>`}
                    </div>
                  </td>
                </tr>
              `).join('')}
          </tbody>
        </table>
      </div>
    </div>

    <div class="admin-card" style="margin-top:24px;">
      <div style="padding:16px 20px;border-bottom:1px solid var(--border-color);">
        <h2 style="margin:0;font-size:16px;font-weight:600;">Geçiş Kuralları (Workflow)</h2>
        <p style="margin:6px 0 0;color:var(--text-muted);font-size:13px;">
          Her satır, bir durumdan sonra hangi kararlara geçilebileceğini tanımlar.
          Değişiklik yaptıktan sonra satırdaki <strong>Kaydet</strong> butonuna tıklayın.
        </p>
      </div>
      <div id="transition-editor" style="padding:16px 20px;">
        ${renderTransitionEditor(types, transMap)}
      </div>
    </div>
  `;

  // ── Type list events ──────────────────────────────────────────────────────
  container.querySelector('#btn-add').addEventListener('click', () =>
    openTypeModal(null, container)
  );

  container.querySelectorAll('.btn-edit-type').forEach(btn =>
    btn.addEventListener('click', () => {
      const t = types.find(x => x.id === Number(btn.dataset.id));
      if (t) openTypeModal(t, container);
    })
  );

  container.querySelectorAll('.btn-toggle-type').forEach(btn =>
    btn.addEventListener('click', async () => {
      const isActive = btn.dataset.active === '1';
      const id = Number(btn.dataset.id);
      try {
        if (isActive) await api.dispositionTypes.delete(id);
        else          await api.dispositionTypes.update(id, { active: true });
        adminToast(`Karar tipi ${isActive ? 'pasife alındı' : 'aktife alındı'}.`, 'success');
        await renderPage(container);
      } catch (e) {
        adminToast('Hata: ' + e.message, 'error');
      }
    })
  );

  // ── Transition editor save events ────────────────────────────────────────
  container.querySelectorAll('.btn-save-transitions').forEach(btn =>
    btn.addEventListener('click', async () => {
      const fromKey  = btn.dataset.from;
      const fromCode = fromKey === '__null__' ? null : fromKey;
      const section  = container.querySelector(`[data-transition-from="${CSS.escape(fromKey)}"]`);
      const checked  = Array.from(section.querySelectorAll('input[type=checkbox]:checked'))
        .map(cb => cb.value);
      try {
        await api.dispositionTransitions.bulkSet(fromCode, checked);
        adminToast('Geçişler kaydedildi.', 'success');
        // Update local transMap to reflect save without full re-render
        transMap[fromKey] = new Set(checked);
      } catch (e) {
        adminToast('Hata: ' + e.message, 'error');
      }
    })
  );
}

function renderTransitionEditor(types, transMap) {
  const activeTypes = types.filter(t => t.active).sort((a, b) => a.sort_order - b.sort_order);

  // from-states: initial (null) + all active non-neutralizing types
  const fromStates = [
    { key: '__null__', label: '— İlk Karar (Henüz Karar Girilmemiş)' },
    ...activeTypes
      .filter(t => !t.is_neutralizing)
      .map(t => ({ key: t.code, label: `${t.label} (${t.code})` })),
  ];

  return fromStates.map(from => {
    const currentSet = transMap[from.key] || new Set();
    const checkboxes = activeTypes.map(t => `
      <label style="display:flex;align-items:center;gap:6px;font-size:13px;cursor:pointer;white-space:nowrap;">
        <input type="checkbox" value="${esc(t.code)}" ${currentSet.has(t.code) ? 'checked' : ''} />
        <span class="disp-badge ${esc(t.css_class)}" style="font-size:11px;">${esc(t.label)}</span>
      </label>
    `).join('');

    return `
      <div class="transition-row" data-transition-from="${esc(from.key)}"
           style="border:1px solid var(--border-color);border-radius:8px;padding:14px 16px;margin-bottom:12px;">
        <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:10px;">
          <strong style="font-size:13px;">${esc(from.label)}</strong>
          <button class="btn btn-sm btn-primary btn-save-transitions" data-from="${esc(from.key)}">
            Kaydet
          </button>
        </div>
        <div style="display:flex;flex-wrap:wrap;gap:10px 20px;">
          ${checkboxes}
        </div>
      </div>
    `;
  }).join('');
}

function openTypeModal(type, container) {
  const isEdit = !!type;
  const html = `
    <div class="modal-header">
      <h2>${isEdit ? 'Karar Tipi Düzenle' : 'Yeni Karar Tipi'}</h2>
      <button class="modal-close-btn" id="modal-close">✕</button>
    </div>
    <div class="modal-body">
      ${!isEdit ? `
        <div class="form-group">
          <label>Kod * <span style="font-size:11px;color:var(--text-muted);">(sonradan değiştirilemez)</span></label>
          <input type="text" id="f-code" class="form-input" placeholder="ör. USE_AS_IS" />
        </div>` : `
        <div class="form-group">
          <label>Kod <span style="font-size:11px;color:var(--text-muted);">(değiştirilemez)</span></label>
          <input type="text" class="form-input" value="${esc(type.code)}" disabled />
        </div>`}
      <div class="form-group">
        <label>Etiket *</label>
        <input type="text" id="f-label" class="form-input" placeholder="ör. Kabul (Spec)"
               value="${isEdit ? esc(type.label) : ''}" />
      </div>
      <div class="form-group">
        <label>CSS Sınıfı <span style="font-size:11px;color:var(--text-muted);">(ör. disp-accepted)</span></label>
        <input type="text" id="f-css" class="form-input" placeholder="disp-accepted"
               value="${isEdit ? esc(type.css_class) : ''}" />
      </div>
      <div class="form-group" style="display:flex;gap:24px;flex-wrap:wrap;">
        <label style="display:flex;align-items:center;gap:8px;cursor:pointer;">
          <input type="checkbox" id="f-neutralizing" ${isEdit && type.is_neutralizing ? 'checked' : ''} />
          Defekti Kapatır (Neutralizing)
        </label>
        <label style="display:flex;align-items:center;gap:8px;cursor:pointer;">
          <input type="checkbox" id="f-initial" ${isEdit && type.is_initial ? 'checked' : ''} />
          Başlangıç Kararı
        </label>
        ${isEdit ? `
        <label style="display:flex;align-items:center;gap:8px;cursor:pointer;">
          <input type="checkbox" id="f-active" ${type.active ? 'checked' : ''} />
          Aktif
        </label>` : ''}
      </div>
      <div class="form-group">
        <label>Sıra Numarası</label>
        <input type="number" id="f-sort" class="form-input" min="0" step="1"
               value="${isEdit ? type.sort_order : 99}" style="max-width:120px;" />
      </div>
      <p id="modal-error" class="text-danger" style="display:none;margin-top:8px;"></p>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary" id="modal-cancel">İptal</button>
      <button class="btn btn-primary" id="modal-save">${isEdit ? 'Kaydet' : 'Oluştur'}</button>
    </div>
  `;

  adminModal.open(html, { onClose: () => renderPage(container) });

  const box = document.getElementById('admin-modal-box');
  box.querySelector('#modal-close').addEventListener('click',  () => adminModal.close());
  box.querySelector('#modal-cancel').addEventListener('click', () => adminModal.close());

  box.querySelector('#modal-save').addEventListener('click', async () => {
    const errEl = box.querySelector('#modal-error');
    errEl.style.display = 'none';

    const label         = box.querySelector('#f-label').value.trim();
    const css_class     = box.querySelector('#f-css').value.trim();
    const is_neutralizing = box.querySelector('#f-neutralizing').checked;
    const is_initial    = box.querySelector('#f-initial').checked;
    const sort_order    = parseInt(box.querySelector('#f-sort').value, 10) || 0;

    if (!label) {
      errEl.textContent = 'Etiket boş olamaz.';
      errEl.style.display = 'block';
      return;
    }

    try {
      if (isEdit) {
        const active = box.querySelector('#f-active').checked;
        await api.dispositionTypes.update(type.id, {
          label, css_class, is_neutralizing, is_initial, sort_order, active
        });
        adminToast('Karar tipi güncellendi.', 'success');
      } else {
        const code = (box.querySelector('#f-code').value.trim()).toUpperCase();
        if (!code) {
          errEl.textContent = 'Kod boş olamaz.';
          errEl.style.display = 'block';
          return;
        }
        await api.dispositionTypes.create({ code, label, css_class, is_neutralizing, is_initial, sort_order });
        adminToast('Karar tipi oluşturuldu.', 'success');
      }
      adminModal.close();
      await renderPage(container);
    } catch (e) {
      errEl.textContent = e.message;
      errEl.style.display = 'block';
    }
  });
}

function esc(s) {
  return String(s ?? '').replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}
