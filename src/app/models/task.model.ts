export interface TaskItem {
  id: string;
  title: string;
  description?: string;
  status: 'Pending' | 'InProgress' | 'Completed';
  dueDate?: string;
  assignedToId?: string;
  assignedToName?: string;
  projectId: string;
  createdAt: string;
  completedAt?: string;
  isOverdue?: boolean;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  dueDate?: string;
  assignedToId?: string;
  projectId: string;
}

export interface UpdateTaskStatusRequest {
  status: 'Pending' | 'InProgress' | 'Completed';
}

export interface DashboardStats {
  totalTasks: number;
  completedTasks: number;
  pendingTasks: number;
  overdueTasks: number;
}
