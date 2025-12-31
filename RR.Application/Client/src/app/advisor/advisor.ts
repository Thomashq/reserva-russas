import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { AdvisorService } from '../advisor/advisor.service';
import { IStudentAdvisor } from '../domain/models/studentadvisor';

@Component({
  selector: 'app-advisor',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './advisor.html',
  styleUrl: './advisor.css',
})
export class AdvisorComponent implements OnInit {
  loading: boolean = false;
  error: string = '';
  list: IStudentAdvisor[] = [];

  filtroStudentId: number | null = null;
  filtroServantId: number | null = null;

  constructor(private advisorService: AdvisorService) {}

  ngOnInit(): void {
    this.LoadAll();
  }

  LoadAll(): void {
    this.loading = true;
    this.error = '';

    this.advisorService.GetAllStudentAdvisors().subscribe({
      next: (res) => {
        this.list = res || [];
        this.loading = false;
      },
      error: (err) => {
        this.error = (err?.error ?? err?.message ?? 'Erro ao carregar');
        this.loading = false;
      },
    });
  }

  Buscar(): void {
    this.loading = true;
    this.error = '';
    this.list = [];

    if (this.filtroStudentId && this.filtroStudentId > 0) {
      this.advisorService.GetStudentAdvisorsByStudentId(this.filtroStudentId).subscribe({
        next: (res) => {
          this.list = res || [];
          this.loading = false;
        },
        error: (err) => {
          this.error = (err?.error ?? err?.message ?? 'Erro ao buscar por StudentId');
          this.loading = false;
        },
      });
      return;
    }

    if (this.filtroServantId && this.filtroServantId > 0) {
      this.advisorService.GetStudentAdvisorsByServantId(this.filtroServantId).subscribe({
        next: (res) => {
          this.list = res || [];
          this.loading = false;
        },
        error: (err) => {
          this.error = (err?.error ?? err?.message ?? 'Erro ao buscar por ServantId');
          this.loading = false;
        },
      });
      return;
    }

    this.LoadAll();
  }

  Limpar(): void {
    this.filtroStudentId = null;
    this.filtroServantId = null;
    this.LoadAll();
  }

  Delete(id: number): void {
    if (!confirm('Desativar essa relação?')) return;

    this.loading = true;
    this.error = '';

    this.advisorService.DeleteStudentAdvisor(id).subscribe({
      next: () => {
        this.LoadAll();
      },
      error: (err) => {
        this.error = (err?.error ?? err?.message ?? 'Erro ao desativar');
        this.loading = false;
      },
    });
  }
}

