import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { LoginDTO, RoleDTO } from '../../interfaces/interfaces';
import { AuthService } from '../../services/auth.service';
import { RoleService } from '../../services/role.service';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    ButtonModule,
    InputTextModule,
    MessagesModule,
    MessageModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private rolService: RoleService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      credencial: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    const credentials: LoginDTO = this.loginForm.value;
    this.authService.login(credentials).subscribe({
      next: (response) => {
        if (response.exito === 1) {
          // Guardar las credenciales del usuario
          localStorage.setItem('user', JSON.stringify(credentials.credencial));

          // Llamar al método para obtener el rol del usuario
          this.getRoleAndMenuOptions(credentials.credencial);

          // Redirigir al dashboard
          this.router.navigate(['/dashboard']);
        } else {
          this.errorMessage = response.mensaje;
        }
      },
      error: () => {
        this.errorMessage = 'Error al intentar iniciar sesión.';
      },
    });
  }
  getRoleAndMenuOptions(credencial: string) {
    this.rolService.getRoleByUsername(credencial).subscribe({
      next: (role: RoleDTO) => {
        // Guardar el rol completo en localStorage
        localStorage.setItem('role', JSON.stringify(role));

        // Guardar solo las opciones de menú (si solo te interesan las rutas)
        const menuOptions = role.rolOpcionesIdOpcions.map(
          (option) => option.route
        );
        localStorage.setItem('menuOptions', JSON.stringify(menuOptions));
      },
      error: () => {
        this.errorMessage = 'Error al obtener el rol del usuario.';
      },
    });
  }
}


