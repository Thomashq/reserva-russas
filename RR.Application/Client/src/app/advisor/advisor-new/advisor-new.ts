import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

import { AdvisorService } from '../advisor.service';

@Component({
  selector: 'app-advisor-new',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './advisor-new.html',
  styleUrl: './advisor-new.css',
})
export class AdvisorNewComponent {
  loading: boolean = false;
  error: string = '';
  ok: string = '';

  studentId: number | null = null;
  servantId: number | null = null;

  constructor(
    private advisorService: AdvisorService,
    private router: Router
  ) {}

  Save(): void {
    this.error = '';
    this.ok = '';

    if (!this.studentId || this.studentId <= 0) {
      this.error = 'StudentId inválido';
      return;
    }

    if (!this.servantId || this.servantId <= 0) {
      this.error = 'ServantId inválido';
      return;
    }

    this.loading = true;

    this.advisorService.CreateStudentAdvisor({
      studentId: this.studentId,
      servantId: this.servantId
    }).subscribe({
      next: (created) => {
        this.loading = false;

        if (!created) {
          this.error = 'Já existe uma relação ativa com esse StudentId e ServantId.';
          return;
        }

        this.ok = 'Relação criada com sucesso.';
        this.router.navigateByUrl('/advisor');
      },
      error: (err) => {
        this.loading = false;
        this.error = (err?.error ?? err?.message ?? 'Erro ao salvar');
      },
    });
  }
}

