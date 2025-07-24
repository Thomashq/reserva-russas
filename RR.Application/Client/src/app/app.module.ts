import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http'; // ✅ necessário para AuthService
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { AuthRoutingModule } from './auth/auth-routing.module';
import { HomePageComponent } from './home-page/home-page.component';
import { LoginComponent } from './auth/auth-login/auth-login.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { HeaderModule } from './header/header.module';
import { FooterModule } from './footer/footer.module';

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    HeaderModule,
    FooterModule,
    AppRoutingModule,
    AuthRoutingModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
