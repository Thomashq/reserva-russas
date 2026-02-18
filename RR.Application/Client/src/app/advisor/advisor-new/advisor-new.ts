import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AdvisorService } from '../advisor.service';
import { AccountLookup } from '../../domain/dto/response/AccountResponse';
import { StudentDto, StudentService } from '../../domain/shared/services/student.service';
import { ServantDto, ServantService } from '../../domain/shared/services/servant.service';

@Component({
  selector: 'app-advisor-new',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,

    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './advisor-new.html',
  styleUrl: './advisor-new.css',
})
export class AdvisorNewComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  loading = false;
  error = '';

  students: StudentDto[] = [];
  servants: ServantDto[] = [];

  selectedStudentId: number | null = null;
  selectedServantId: number | null = null;

  constructor(
    private advisorService: AdvisorService,
    private studentApi: StudentService,
    private servantApi: ServantService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAll();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadAll(): void {
    this.loading = true;
    this.error = '';

    this.studentApi.GetAll().pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => this.students = res ?? [],
      error: () => this.students = []
    });

    this.servantApi.GetAll().pipe(
      takeUntil(this.destroy$),
      finalize(() => this.loading = false)
    ).subscribe({
      next: (res) => this.servants = res ?? [],
      error: () => this.servants = []
    });
  }

  private getAccountFromStudent(s: StudentDto): AccountLookup | null {
    return (s.Account ?? s.account ?? null) as any;
  }

  private getAccountFromServant(s: ServantDto): AccountLookup | null {
    return (s.Account ?? s.account ?? null) as any;
  }

  displayStudent(s: StudentDto): string {
    const a = this.getAccountFromStudent(s);
    if (a) {
      const name = (a.UserName ?? '').trim();
      const mail = (a.Mail ?? '').trim();
      if (name && mail) return `${name} (${mail})`;
      return name || mail || `#${a.Id}`;
    }
    return `Conta #${s.AccountId}`;
  }

  displayServant(s: ServantDto): string {
    const a = this.getAccountFromServant(s);
    if (a) {
      const name = (a.UserName ?? '').trim();
      const mail = (a.Mail ?? '').trim();
      console.log(name, mail)
      if (name && mail) return `${name} (${mail})`;
      return name || mail || `#${a.Id}`;
    }
    return `Conta #${s.AccountId}`;
  }

  clearAll(): void {
    this.error = '';
    this.selectedStudentId = null;
    this.selectedServantId = null;
  }

  save(): void {
    this.error = '';

    if (!this.selectedStudentId) {
      this.error = 'Selecione o estudante.';
      return;
    }

    if (!this.selectedServantId) {
      this.error = 'Selecione o servidor.';
      return;
    }

    this.loading = true;

    this.advisorService.CreateStudentAdvisor({
      studentId: this.selectedStudentId,
      servantId: this.selectedServantId
    }).pipe(
      takeUntil(this.destroy$),
      finalize(() => this.loading = false)
    ).subscribe({
      next: (created) => {
        if (!created) {
          this.error = 'Já existe uma relação ativa para esse estudante e servidor.';
          return;
        }
        this.router.navigateByUrl('/advisor');
      },
      error: (err) => {
        this.error = (err?.error ?? err?.message ?? 'Erro ao salvar');
      }
    });
  }
}

