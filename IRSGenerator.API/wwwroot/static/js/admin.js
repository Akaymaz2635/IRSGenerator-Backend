/**
 * admin.js — Admin SPA entry point
 * Hash-based routing: #/dashboard, #/users, #/projects, etc.
 */

import { adminDashboardPage }        from './pages/admin-dashboard.js';
import { adminUsersPage }            from './pages/admin-users.js';
import { adminProjectsPage }         from './pages/admin-projects.js';
import { adminDefectTypesPage }      from './pages/admin-defect-types.js';
import { adminDefectFieldsPage }     from './pages/admin-defect-fields.js';
import { adminDispositionTypesPage } from './pages/admin-disposition-types.js';
import { adminConfigPage }           from './pages/admin-config.js';
import { adminBackupPage }           from './pages/admin-backup.js';
import { session }                   from './session.js';

// Guard: only admin role may access this panel
if (!session.isAdmin()) {
  document.body.innerHTML = '<div style="padding:40px;font-family:sans-serif;color:#dc3545;font-size:16px;">Erişim reddedildi. Bu sayfaya yalnızca admin girebilir.</div>';
  throw new Error('Access denied — admin only');
}

// ── Router ──────────────────────────────────────────────────────────────────
const routes = {
  '/dashboard':         adminDashboardPage,
  '/users':             adminUsersPage,
  '/projects':          adminProjectsPage,
  '/defect-types':      adminDefectTypesPage,
  '/defect-fields':     adminDefectFieldsPage,
  '/disposition-types': adminDispositionTypesPage,
  '/config':            adminConfigPage,
  '/backup':            adminBackupPage,
};

function currentPath() {
  return location.hash.replace(/^#/, '') || '/dashboard';
}

async function navigate(path) {
  if (!path.startsWith('/')) path = '/' + path;
  location.hash = '#' + path;
}
window.adminNavigate = navigate;

async function render() {
  const path    = currentPath();
  const handler = routes[path] || adminDashboardPage;
  const content = document.getElementById('admin-content');
  content.innerHTML = '<div class="admin-loading">Yükleniyor…</div>';

  try {
    await handler(content);
  } catch (e) {
    content.innerHTML = `<div class="admin-empty" style="color:var(--danger)">Hata: ${e.message}</div>`;
  }

  updateNav(path);
}

function updateNav(path) {
  document.querySelectorAll('#admin-nav a').forEach(link => {
    const page = link.dataset.page;
    let active = false;
    if (page === 'dashboard')     active = path === '/dashboard' || path === '/';
    else                          active = path.startsWith('/' + page);
    link.classList.toggle('active', active);
  });
}

window.addEventListener('hashchange', render);
render();

// ── Theme ─────────────────────────────────────────────────────────────────────
function applyTheme(theme) {
  document.documentElement.setAttribute('data-theme', theme);
  const btn = document.getElementById('theme-toggle-btn');
  if (!btn) return;
  const icon  = btn.querySelector('.theme-toggle-icon');
  const label = btn.querySelector('.theme-toggle-label');
  if (theme === 'light') {
    icon.textContent  = '🌙';
    label.textContent = 'Koyu Tema';
  } else {
    icon.textContent  = '☀️';
    label.textContent = 'Açık Tema';
  }
}

(function initTheme() {
  applyTheme(localStorage.getItem('qs-theme') || 'dark');
})();

document.getElementById('theme-toggle-btn').addEventListener('click', () => {
  const current = document.documentElement.getAttribute('data-theme') || 'dark';
  const next = current === 'dark' ? 'light' : 'dark';
  localStorage.setItem('qs-theme', next);
  applyTheme(next);
});

// ── Toast ────────────────────────────────────────────────────────────────────
window.adminToast = function(msg, type = 'info') {
  const container = document.getElementById('admin-toast-container');
  const el = document.createElement('div');
  el.className = `toast toast-${type}`;
  el.textContent = msg;
  container.appendChild(el);
  setTimeout(() => {
    el.classList.add('hiding');
    el.addEventListener('animationend', () => el.remove(), { once: true });
    setTimeout(() => el.remove(), 400);
  }, 3200);
};

// ── Modal ────────────────────────────────────────────────────────────────────
window.adminModal = {
  _onClose: null,
  open(html, { wide = false, onClose = null } = {}) {
    const overlay = document.getElementById('admin-modal-overlay');
    const box     = document.getElementById('admin-modal-box');
    box.className = 'modal-box' + (wide ? ' modal-wide' : '');
    box.innerHTML = html;
    overlay.classList.remove('hidden');
    this._onClose = onClose;

    overlay.onclick = (e) => { if (e.target === overlay) this.close(); };
    this._esc = (e) => { if (e.key === 'Escape') this.close(); };
    window.addEventListener('keydown', this._esc);
  },
  close() {
    const overlay = document.getElementById('admin-modal-overlay');
    overlay.classList.add('hidden');
    overlay.onclick = null;
    if (this._esc) { window.removeEventListener('keydown', this._esc); this._esc = null; }
    if (this._onClose) { this._onClose(); this._onClose = null; }
  },
};
