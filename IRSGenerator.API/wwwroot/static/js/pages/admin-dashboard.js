import { api } from '../api.js';

export async function adminDashboardPage(container) {
  // Fetch counts in parallel
  const [users, projects, defectTypes, inspections] = await Promise.allSettled([
    api.users.list(true),
    api.projects.list(true),
    api.defectTypes.list(true),
    api.inspections.list({}),
  ]);

  const get = (r) => r.status === 'fulfilled' ? r.value : [];

  const allUsers       = get(users);
  const allProjects    = get(projects);
  const allDefectTypes = get(defectTypes);
  const allInspections = get(inspections);

  const activeUsers    = allUsers.filter(u => u.active).length;
  const activeProjects = allProjects.filter(p => p.active).length;
  const openInsp       = allInspections.filter(i => i.status === 'open').length;
  const closedInsp     = allInspections.filter(i => i.status === 'closed').length;

  container.innerHTML = `
    <div class="admin-page-header">
      <div>
        <h1>Dashboard</h1>
        <p>Sistemin genel durumuna bakış</p>
      </div>
    </div>

    <div class="admin-stats">
      <div class="stat-card">
        <div class="stat-value">${activeUsers}</div>
        <div class="stat-label">Aktif Kullanıcı</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">${activeProjects}</div>
        <div class="stat-label">Aktif Proje</div>
      </div>
      <div class="stat-card">
        <div class="stat-value" style="color:var(--warning)">${openInsp}</div>
        <div class="stat-label">Açık Muayene</div>
      </div>
      <div class="stat-card">
        <div class="stat-value" style="color:var(--success)">${closedInsp}</div>
        <div class="stat-label">Kapalı Muayene</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">${allDefectTypes.filter(d => d.active).length}</div>
        <div class="stat-label">Aktif Hata Tipi</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">${allInspections.length}</div>
        <div class="stat-label">Toplam Muayene</div>
      </div>
    </div>

    <div class="admin-card" style="padding:24px;">
      <h3 style="font-size:14px;font-weight:600;color:var(--text-secondary);margin-bottom:16px;">Hızlı Erişim</h3>
      <div style="display:flex;flex-wrap:wrap;gap:10px;">
        <a href="#/users"         class="btn btn-secondary">👥 Kullanıcılar</a>
        <a href="#/projects"      class="btn btn-secondary">🏗 Projeler</a>
        <a href="#/defect-types"  class="btn btn-secondary">🔧 Hata Tipleri</a>
        <a href="#/defect-fields" class="btn btn-secondary">📋 Hata Alanları</a>
        <a href="#/config"        class="btn btn-secondary">⚙️ Yapılandırma</a>
      </div>
    </div>
  `;
}
