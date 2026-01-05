//TODO: testar isso aqui
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatTableModule } from '@angular/material/table';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { Subject, finalize, takeUntil } from 'rxjs';

import { AdvisorService } from '../advisor/advisor.service';
import { IStudentAdvisor } from '../domain/models/studentadvisor';
import { AccountLookup } from '../domain/dto/response/AccountResponse';
import { AccountLookupService } from '../domain/shared/services/accountLookUp.service';
import { StudentService } from '../domain/shared/services/student.service';
import { ServantService } from '../domain/shared/services/servant.service';
import { EAccountPermission } from '../domain/enum/EAccountPermission';

@Component({
  selector: 'app-advisor',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatAutocompleteModule,
    MatTableModule,
    MatProgressBarModule
  ],
  templateUrl: './advisor.html',
  styleUrls: ['./advisor.css'],
})
export class AdvisorComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  loading = false;
  error = '';

  list: IStudentAdvisor[] = [];
  displayedColumns: string[] = ['id', 'studentId', 'servantId', 'active', 'actions'];

  studentText = '';
  servantText = '';

  studentOptions: AccountLookup[] = [];
  servantOptions: AccountLookup[] = [];

  selectedStudentAccountId: number | null = null;
  selectedServantAccountId: number | null = null;

  resolvedStudentId: number | null = null;
  resolvedServantId: number | null = null;

  constructor(
    private advisorService: AdvisorService,
    private accountLookup: AccountLookupService,
    private studentApi: StudentService,
    private servantApi: ServantService
  ) {}

  ngOnInit(): void {
    this.loadAll();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  displayAcc(acc: AccountLookup | null): string {
    if (!acc) return '';
    const name = (acc.UserName ?? '').trim();
    const mail = (acc.Mail ?? '').trim();
    if (name && mail) return `${name} (${mail})`;
    return name || mail || `#${acc.Id}`;
  }

  onStudentType(): void {
    const q = (this.studentText ?? '').trim();

    this.studentOptions = [];
    this.selectedStudentAccountId = null;
    this.resolvedStudentId = null;

    if (!q || q.length < 2) return;

    this.accountLookup.Search(q, EAccountPermission.Student, 20)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => this.studentOptions = res ?? [],
        error: () => this.studentOptions = []
      });
  }

  onServantType(): void {
    const q = (this.servantText ?? '').trim();

    this.servantOptions = [];
    this.selectedServantAccountId = null;
    this.resolvedServantId = null;

    if (!q || q.length < 2) return;

    this.accountLookup.Search(q, EAccountPermission.Servant, 20)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => this.servantOptions = res ?? [],
        error: () => this.servantOptions = []
      });
  }

  onPickStudent(ev: MatAutocompleteSelectedEvent): void {
    const acc = ev.option.value as AccountLookup;
    if (!acc?.Id) return;

    this.studentText = this.displayAcc(acc);
    this.selectedStudentAccountId = acc.Id;
    this.resolvedStudentId = null;

    this.studentApi.GetByAccountId(acc.Id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (st) => this.resolvedStudentId = st?.Id ?? null,
        error: () => this.resolvedStudentId = null
      });
  }

  onPickServant(ev: MatAutocompleteSelectedEvent): void {
    const acc = ev.option.value as AccountLookup;
    if (!acc?.Id) return;

    this.servantText = this.displayAcc(acc);
    this.selectedServantAccountId = acc.Id;
    this.resolvedServantId = null;

    this.servantApi.GetByAccountId(acc.Id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (sv) => this.resolvedServantId = sv?.Id ?? null,
        error: () => this.resolvedServantId = null
      });
  }

  search(): void {
    this.error = '';
    this.list = [];
    this.loading = true;

    if (this.resolvedStudentId && this.resolvedStudentId > 0) {
      this.advisorService.GetStudentAdvisorsByStudentId(this.resolvedStudentId).pipe(
        finalize(() => this.loading = false),
        takeUntil(this.destroy$)
      ).subscribe({
        next: (res) => this.list = res ?? [],
        error: (err) => this.error = (err?.error ?? err?.message ?? 'Erro ao buscar por aluno')
      });
      return;
    }

    if (this.resolvedServantId && this.resolvedServantId > 0) {
      this.advisorService.GetStudentAdvisorsByServantId(this.resolvedServantId).pipe(
        finalize(() => this.loading = false),
        takeUntil(this.destroy$)
      ).subscribe({
        next: (res) => this.list = res ?? [],
        error: (err) => this.error = (err?.error ?? err?.message ?? 'Erro ao buscar por servidor')
      });
      return;
    }

    this.loadAll();
  }

  clear(): void {
    this.studentText = '';
    this.servantText = '';
    this.studentOptions = [];
    this.servantOptions = [];
    this.selectedStudentAccountId = null;
    this.selectedServantAccountId = null;
    this.resolvedStudentId = null;
    this.resolvedServantId = null;
    this.loadAll();
  }

  loadAll(): void {
    this.loading = true;
    this.error = '';

    this.advisorService.GetAllStudentAdvisors().pipe(
      finalize(() => this.loading = false),
      takeUntil(this.destroy$)
    ).subscribe({
      next: (res) => this.list = res ?? [],
      error: (err) => this.error = (err?.error ?? err?.message ?? 'Erro ao carregar')
    });
  }

  delete(id: number): void {
    const ok = window.confirm('Desativar essa relação?');
    if (!ok) return;

    this.loading = true;
    this.error = '';

    this.advisorService.DeleteStudentAdvisor(id).pipe(
      finalize(() => this.loading = false),
      takeUntil(this.destroy$)
    ).subscribe({
      next: () => this.search(),
      error: (err) => this.error = (err?.error ?? err?.message ?? 'Erro ao desativar')
    });
  }

  isActive(item: any): boolean {
    return !!(item?.isActive ?? item?.IsActive ?? item?.active);
  }
}
