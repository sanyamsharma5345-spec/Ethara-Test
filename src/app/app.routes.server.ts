import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  // Static routes - prerender
  {
    path: 'login',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'signup',
    renderMode: RenderMode.Prerender
  },
  // Dynamic routes with parameters - use Server instead of prerender
  {
    path: 'dashboard',
    renderMode: RenderMode.Server
  },
  {
    path: 'projects',
    renderMode: RenderMode.Server
  },
  {
    path: 'tasks',
    renderMode: RenderMode.Server
  },
  {
    path: 'tasks/:projectId',
    renderMode: RenderMode.Server
  },
  // Catch-all fallback
  {
    path: '**',
    renderMode: RenderMode.Server
  }
];
