import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { InputSwitchModule } from 'primeng/inputswitch';
import { InputTextModule } from 'primeng/inputtext';
import { RippleModule } from 'primeng/ripple';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { ActualizarUsuarioDto } from '../../interfaces/interfaces';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-user-table',
  imports : [TableModule,
    ToastModule,
    ConfirmDialogModule,
    FileUploadModule,
    ButtonModule,
    RippleModule,
    InputTextModule,
    DialogModule,
    CalendarModule,
    InputSwitchModule,
    TagModule,
    DropdownModule,
    TooltipModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [MessageService,ConfirmationService],
  templateUrl: './user-table.component.html',
  styleUrl: './user-table.component.css',
  standalone: true,

})
export class UserTableComponent {
  users: ActualizarUsuarioDto[] = [];
  selectedUsers: ActualizarUsuarioDto[] = [];
  user: ActualizarUsuarioDto = {
    UserName : "",
    Apellidos : "",
    Identificacion : "",
    Nombres : "",
    active :false
  };
  submitted: boolean = false;
  userDialog: boolean = false;
  isAdmin: boolean = false;
  loading: boolean = true;
  
  globalFilter: string = '';

  constructor(
    private authService: AuthService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {}
  
  ngOnInit() {
    // Verificar si el usuario actual es administrador
    // this.isAdmin = this.authService.currentUser.role === 'ADMIN';
    this.isAdmin = true; // Solo para demostración
    
    this.loadUsers();
  }
  
  loadUsers() {
    this.loading = true;
    this.authService.getUsers().subscribe(
      (data) => {
        this.users = data;
        this.loading = false;
      },
      (error) => {
        this.messageService.add({severity:'error', summary: 'Error', detail: 'No se pudieron cargar los usuarios'});
        this.loading = false;
      }
    );
  }
  
  openNew() {
    this.user = {
      UserName : "",
      Apellidos : "",
      Identificacion : "",
      Nombres : "",
      active:false
    };
    this.submitted = false;
    this.userDialog = true;
  }
  
  editUser(user: ActualizarUsuarioDto) {
    // Verificar si el usuario tiene permisos para editar
    if (!this.isAdmin && user.Identificacion !== this.getCurrentUserId()) {
      this.messageService.add({severity:'warn', summary: 'Acceso denegado', detail: 'No tienes permisos para editar este usuario'});
      return;
    }
    
    this.user = {...user};
    this.userDialog = true;
  }
  
  deleteUser(user: ActualizarUsuarioDto) {
    // Verificar si el usuario tiene permisos para eliminar
    if (!this.isAdmin) {
      this.messageService.add({severity:'warn', summary: 'Acceso denegado', detail: 'No tienes permisos para eliminar usuarios'});
      return;
    }
    
    // this.confirmationService.confirm({
    //   message: '¿Estás seguro de que deseas eliminar el usuario ' + user.username + '?',
    //   header: 'Confirmar',
    //   icon: 'pi pi-exclamation-triangle',
    //   accept: () => {
    //     this.authService.(user.id).subscribe(
    //       () => {
    //         this.users = this.users.filter(val => val.id !== user.id);
    //         this.messageService.add({severity:'success', summary: 'Éxito', detail: 'Usuario eliminado', life: 3000});
    //       },
    //       (error) => {
    //         this.messageService.add({severity:'error', summary: 'Error', detail: 'No se pudo eliminar el usuario'});
    //       }
    //     );
    //   }
    // });
  }
  
  hideDialog() {
    this.userDialog = false;
    this.submitted = false;
  }
  
  saveUser() {
    this.submitted = true;
    
    // Validación de campos requeridos
    if (!this.user.UserName || !this.user.Identificacion) {
      return;
    }
    
    if (this.user.Identificacion) {
      // Actualizar usuario existente
      this.authService.updateProfile(this.user).subscribe(
        (result) => {
          // Actualizar el usuario en la lista
          const index = this.findIndexById(this.user.Identificacion);
          if (index !== -1) {
            this.users[index] = result;
          }
          this.messageService.add({severity:'success', summary: 'Éxito', detail: 'Usuario actualizado', life: 3000});
        },
        (error) => {
          this.messageService.add({severity:'error', summary: 'Error', detail: 'No se pudo actualizar el usuario'});
        }
      );
    }
    
    this.userDialog = false;
    this.user = {
      UserName : "",
      Apellidos : "",
      Identificacion : "",
      Nombres : "",
      active:false
    };

  }
  
  // Cambiar el estado del usuario (activo/inactivo)
  toggleUserStatus(user: ActualizarUsuarioDto) {
    if (!this.isAdmin) {
      this.messageService.add({severity:'warn', summary: 'Acceso denegado', detail: 'No tienes permisos para cambiar el estado de los usuarios'});
      return;
    }
    
    const updatedUser = {...user, active: !user.active};
    
    this.authService.updateProfile(updatedUser).subscribe(
      (result) => {
        const index = this.findIndexById(user.Identificacion);
        if (index !== -1) {
          this.users[index] = result;
        }
        this.messageService.add({
          severity:'success', 
          summary: 'Éxito', 
          detail: `Usuario ${result.active ? 'activado' : 'desactivado'}`, 
          life: 3000
        });
      },
      (error) => {
        this.messageService.add({severity:'error', summary: 'Error', detail: 'No se pudo actualizar el estado del usuario'});
      }
    );
  }
  
  // Cargar usuarios desde archivo Excel
  importUsers(event: { files: any[]; }) {
    if (!this.isAdmin) {
      this.messageService.add({severity:'warn', summary: 'Acceso denegado', detail: 'No tienes permisos para importar usuarios'});
      return;
    }
    
    const file = event.files[0];
    const formData = new FormData();
    formData.append('file', file);
    
    // this.authService.importUsers(formData).subscribe(
    //   (result) => {
    //     this.loadUsers(); // Recargar la lista de usuarios
    //     this.messageService.add({severity:'success', summary: 'Éxito', detail: `${result.importedCount} usuarios importados correctamente`, life: 3000});
    //   },
    //   (error) => {
    //     this.messageService.add({severity:'error', summary: 'Error', detail: 'No se pudieron importar los usuarios'});
    //   }
    // );
  }
  
  // Métodos auxiliares
  findIndexById(id: string): number {
    let index = -1;
    for (let i = 0; i < this.users.length; i++) {
      if (this.users[i].Identificacion === id) {
        index = i;
        break;
      }
    }
    return index;
  }
  
  getCurrentUserId(): string {
    
    return 'current-user-id';
  }
  
  // Exportar usuarios a Excel
  exportExcel() {
    import('xlsx').then(xlsx => {
      const worksheet = xlsx.utils.json_to_sheet(this.users);
      const workbook = { Sheets: { 'Usuarios': worksheet }, SheetNames: ['Usuarios'] };
      const excelBuffer: any = xlsx.write(workbook, { bookType: 'xlsx', type: 'array' });
      this.saveAsExcelFile(excelBuffer, 'usuarios');
    });
  }
  
  saveAsExcelFile(buffer: any, fileName: string): void {
    import('file-saver').then(FileSaver => {
      let EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
      let EXCEL_EXTENSION = '.xlsx';
      const data: Blob = new Blob([buffer], {type: EXCEL_TYPE});
      FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
    });
  }
}
