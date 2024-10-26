import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  public isLoggedIn = false;
  public forecasts: WeatherForecast[] = [];
  public data: WeatherForecast[] = [];
  public userProfile: KeycloakProfile | null = null;
  public isUnauthorized = false;

  constructor(private http: HttpClient, private readonly keycloak: KeycloakService) {}

  async ngOnInit() {
    this.isLoggedIn = await this.keycloak.isLoggedIn();

    if (this.isLoggedIn) {
      this.userProfile = await this.keycloak.loadUserProfile();
    }

    await this.getForecasts();

    await this.getData();
  }

  getForecasts() {
    this.http.get<WeatherForecast[]>('/weatherforecast/forecast').subscribe(
      (result) => {
        this.forecasts = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  getData() {
    this.http.get<WeatherForecast[]>('/weatherforecast/data').subscribe(
      (result) => {
        this.data = result;
        this.isUnauthorized = false;
      },
      (error) => {
        if (error.status == 403)
          this.isUnauthorized = true;
        else
          this.isUnauthorized = false;

        console.error(error);
      }
    );
  }

  public login() {
    this.keycloak.login();
  }

  public logout() {
    this.keycloak.logout();
  }

  title = 'securityworkshop.client';
}
