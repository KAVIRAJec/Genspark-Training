import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { ToastComponent } from './Components/Toast/toast.component';
import { ToastService } from './Components/Toast/toast.service';
import { AsyncPipe, CommonModule } from '@angular/common';
import { Navbar } from './Components/navbar/navbar';
import AOS from 'aos';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToastComponent, CommonModule, AsyncPipe, Navbar],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit{
  protected title = 'Migration-FE';
  toast$;
  constructor(public toastService: ToastService, private router: Router) {
    this.toast$ = this.toastService.toast$;
  }

  ngOnInit(): void {
    AOS.init();
  }

  isAuthPage(): boolean {
    return this.router.url.startsWith('/login') || this.router.url.startsWith('/register');
  }
}
