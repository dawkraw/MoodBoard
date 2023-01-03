import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../services/auth.service";
import {StorageService} from "../services/storage.service";
import {ModalService} from "../services/modal.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;

  errorMessage = '';

  constructor(private authService: AuthService, private storageService: StorageService, private router: Router) {}

  get username() {
    return this.loginForm.get('username')!
  }

  get password() {
    return this.loginForm.get('password')!
  }

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required)
    });
  }

  submitForm(): void {
    this.authService.login(this.username.value, this.password.value).subscribe({
      next: data => {
        console.log(data);
        this.storageService.saveUser(data);
        this.router.navigate(["home"]);
      },
      error: data => {
        this.errorMessage = data.error.errorMessage;
      }
    })
  }
}
