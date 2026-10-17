import { Component } from '@angular/core';
import { SignOn } from '../../features/sign-on/sign-on';

@Component({
  imports: [SignOn],
  selector: 'app-login',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class Login {}
