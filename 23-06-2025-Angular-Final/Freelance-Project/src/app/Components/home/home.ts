import { Component, OnInit, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { Store } from '@ngrx/store';
import { selectClientPagination } from '../../NgRx/Client/client.selector';
import { selectProjectPagination } from '../../NgRx/Project/project.selector';
import { selectFreelancerPagination } from '../../NgRx/Freelancer/freelancer.selector';
import * as ClientActions from '../../NgRx/Client/client.actions';
import * as ProjectActions from '../../NgRx/Project/project.actions';
import * as FreelancerActions from '../../NgRx/Freelancer/freelancer.actions';
import { NgOptimizedImage } from '@angular/common';
import { interval, takeWhile } from 'rxjs';

@Component({
  selector: 'app-home',
  imports: [NgOptimizedImage],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class Home implements OnInit, AfterViewInit {
  animatedProjectCount: number = 0;
  animatedFreelancerCount: number = 0;
  animatedClientCount: number = 0;
  public currentYear: number = new Date().getFullYear();

  private projectCountTarget = 0;
  private freelancerCountTarget = 0;
  private clientCountTarget = 0;
  private hasAnimated = false;

  @ViewChild('countSection') countSection!: ElementRef;

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.store.dispatch(ClientActions.loadClients({ page: 1, pageSize: 1 }));
    this.store.dispatch(ProjectActions.loadProjects({ page: 1, pageSize: 1 }));
    this.store.dispatch(FreelancerActions.loadFreelancers({ page: 1, pageSize: 1 }));

    this.store.select(selectClientPagination).subscribe(pagination => {
      if (pagination && pagination.totalRecords) {
        this.clientCountTarget = pagination.totalRecords;
        this.tryAnimateCounts();
      }
    });
    this.store.select(selectProjectPagination).subscribe(pagination => {
      if (pagination && pagination.totalRecords) {
        this.projectCountTarget = pagination.totalRecords;
        this.tryAnimateCounts();
      }
    });
    this.store.select(selectFreelancerPagination).subscribe(pagination => {
      if (pagination && pagination.totalRecords) {
        this.freelancerCountTarget = pagination.totalRecords;
        this.tryAnimateCounts();
      }
    });
  }

  ngAfterViewInit(): void {
    if (this.countSection) {
      const observer = new IntersectionObserver(entries => {
        const entry = entries[0];
        if (entry.isIntersecting && entry.intersectionRatio > 0.9) {
          // Animate every time section is at least 90% visible
          this.animateCount(this.projectCountTarget, 'animatedProjectCount');
          this.animateCount(this.clientCountTarget, 'animatedClientCount');
          this.animateCount(this.freelancerCountTarget, 'animatedFreelancerCount');
        } else if (!entry.isIntersecting || entry.intersectionRatio < 0.2) {
          // Reset counts when section is mostly out of view
          this.animatedProjectCount = 0;
          this.animatedClientCount = 0;
          this.animatedFreelancerCount = 0;
        }
      }, { threshold: [0.2, 0.9] }); // 20% and 90% thresholds
      observer.observe(this.countSection.nativeElement);
    }
  }

  tryAnimateCounts() {
    if (this.hasAnimated) return;
    if (this.projectCountTarget && this.clientCountTarget && this.freelancerCountTarget && this.countSection) {
      // If already in view, animate
      const rect = this.countSection.nativeElement.getBoundingClientRect();
      if (rect.top < window.innerHeight && rect.bottom > 0) {
        this.hasAnimated = true;
        this.animateCount(this.projectCountTarget, 'animatedProjectCount');
        this.animateCount(this.clientCountTarget, 'animatedClientCount');
        this.animateCount(this.freelancerCountTarget, 'animatedFreelancerCount');
      }
    }
  }

  animateCount(target: number, property: 'animatedProjectCount' | 'animatedFreelancerCount' | 'animatedClientCount', duration = 3000) {
    const stepTime = 200; // ms per tick
    const steps = Math.ceil(duration / stepTime);
    let current = 0;
    const increment = Math.ceil(target / steps);

    const intervalId = setInterval(() => {
      current += increment;
      if (current >= target) {
        this[property] = target;
        clearInterval(intervalId);
      } else {
        this[property] = current;
      }
    }, stepTime);
  }
}