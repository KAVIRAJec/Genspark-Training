import { Component, OnInit } from '@angular/core';
import { UserService } from '../Services/User.service';
import { UserModel } from '../Models/User';
import { CommonModule } from '@angular/common';
import Chart from 'chart.js/auto';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  imports: [CommonModule, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit {
  users: UserModel[] = [];
  searchText: string = '';
  genderStats = { male: 0, female: 0 };
  roleStats: { [role: string]: number } = {};
  stateStats: { [state: string]: number } = {};
  genderChart: Chart | undefined;
  roleChart: Chart | undefined;

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.userService.getAllUsers().subscribe(users => {
      this.users = users;
      this.calculateStats();
      this.renderCharts();
      this.renderMap();
    });
  }

  onSearchChange() {
  this.calculateStats(this.filteredUsers);
  this.renderCharts();
}

  
  get filteredUsers(): UserModel[] {
  if (!this.searchText.trim()) return this.users;
  const text = this.searchText.toLowerCase();
  return this.users.filter(user =>
    user.firstName?.toLowerCase().includes(text) ||
    user.lastName?.toLowerCase().includes(text) ||
    user.email?.toLowerCase().includes(text) ||
    user.role?.toLowerCase().includes(text) ||
    user.gender?.toLowerCase().includes(text) ||
    user.state?.toLowerCase().includes(text)
  );
}

  calculateStats(users: UserModel[] = this.users) {
    this.genderStats = { male: 0, female: 0 };
    this.roleStats = {};
    this.stateStats = {};
    for (const user of users) {
      // Gender
      if ((user as any).gender === 'female') this.genderStats.female++;
      else this.genderStats.male++;
      // Role
      const role = (user as any).role;
      this.roleStats[role] = (this.roleStats[role] || 0) + 1;
      // State
      if (user.state) this.stateStats[user.state] = (this.stateStats[user.state] || 0) + 1;
    }
  }

  renderCharts() {
    if (this.genderChart) this.genderChart.destroy();
    if (this.roleChart) this.roleChart.destroy();
    // Gender Pie Chart
    const genderCtx = document.getElementById('genderChart') as HTMLCanvasElement;
    if (genderCtx) {
      this.genderChart = new Chart(genderCtx, {
        type: 'pie',
        data: {
          labels: ['Female', 'Male'],
          datasets: [{
            data: [this.genderStats.female, this.genderStats.male],
            backgroundColor: ['#a6a4a4', '#1976d2']
          }]
        },
        options: {
          plugins: { legend: { display: false } },
          responsive: true,
        }
      });
    }
    // Role Bar Chart
    const roleCtx = document.getElementById('roleChart') as HTMLCanvasElement;
    this.roleChart = new Chart(roleCtx, {
        type: 'bar',
        data: {
          labels: Object.keys(this.roleStats),
          datasets: [{
            data: Object.values(this.roleStats),
            backgroundColor: '#1976d2',
          }]
        },
        options: {
          plugins: { legend: { display: false } },
          responsive: true,
          scales: {
            x: { title: { display: false } },
            y: { beginAtZero: true }
          }
        }
      });
    }

    renderMap() {
      // TODO: Implement map rendering logic here if needed.
    }
  }