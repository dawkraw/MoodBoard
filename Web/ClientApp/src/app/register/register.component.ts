import {Component, OnInit} from '@angular/core';
import {AuthService} from "../services/auth.service";
import {AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators} from "@angular/forms";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  isRegisterSuccess!: boolean;
  registerErrors!: Array<string>;

  constructor(private authService: AuthService) {
  }

  get username() {
    return this.registerForm.get('username')!
  }

  get password() {
    return this.registerForm.get('password')!
  }

  get confirm_password() {
    return this.registerForm.get('confirm_password')!
  }

  ngOnInit(): void {
    this.registerForm = new FormGroup({
      username: new FormControl('',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(20)
        ]),
      password: new FormControl('',
        [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z])(?=.*[#?!@$%^&*-]).{6,}$")
        ]),
      confirm_password: new FormControl('',
        [
          Validators.required,
          this.sameValueAsPassword()
        ]
      )
    });
  }

  sameValueAsPassword(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      let isEqual = control.value === this.registerForm?.value?.['password'];
      return !isEqual ? {isEqual: true} : null;
    }
  }

  submitForm(): void {
    this.authService.register(this.username.value, this.password.value).subscribe({
      next: data => {
        this.isRegisterSuccess = true;
      },
      error: error => {
        console.log(error);
        this.isRegisterSuccess = false;
        this.registerErrors = error.error;
      }
    })
  }
}
