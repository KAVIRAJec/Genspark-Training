import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <footer class="footer">
      <div class="footer-content">
        <p>&copy; {{ currentYear }} VideoHub Portal. Built with Angular & .NET</p>
        <p class="footer-subtitle">Professional Training Video Streaming Platform</p>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 2rem;
      margin-top: auto;
      text-align: center;
      backdrop-filter: blur(10px);
    }

    .footer-content {
      max-width: 1200px;
      margin: 0 auto;
    }

    .footer-content p {
      margin: 0.25rem 0;
      opacity: 0.9;
    }

    .footer-subtitle {
      font-size: 0.9rem;
      opacity: 0.7;
    }

    @media (max-width: 768px) {
      .footer {
        padding: 1.5rem;
      }
      
      .footer-content p {
        font-size: 0.9rem;
      }
    }
  `]
})
export class FooterComponent {
  currentYear = new Date().getFullYear();
}
