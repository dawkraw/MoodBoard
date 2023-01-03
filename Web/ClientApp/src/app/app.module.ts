import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {HomeComponent} from './home/home.component';
import {LoginComponent} from './login/login.component';
import {BoardComponent} from './board/board.component';
import {httpInterceptorProviders} from "./http.interceptor";
import {RegisterComponent} from './register/register.component';
import {AuthActivatorService} from "./services/auth-activator.service";
import {BoardListComponent} from './board-list/board-list.component';
import {BoardItemComponent} from './board-item/board-item.component';
import {BoardFormComponent} from './board-form/board-form.component';
import {BoardItemFormComponent} from './board-item-form/board-item-form.component';
import {SearchComponent} from './search/search.component';
import {ModalComponent} from './modal/modal.component';
import {BoardDeletePromptComponent} from './board-delete-prompt/board-delete-prompt.component';
import {BoardMembersComponent} from './board-members/board-members.component';
import {BoardItemListComponent} from './board-item-list/board-item-list.component';
import {ItemCommentsComponent} from './item-comments/item-comments.component';
import {registerLocaleData} from "@angular/common";
import localeFr from '@angular/common/locales/fr';

registerLocaleData(localeFr);

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    LoginComponent,
    BoardComponent,
    RegisterComponent,
    BoardListComponent,
    BoardItemComponent,
    BoardFormComponent,
    BoardItemFormComponent,
    SearchComponent,
    ModalComponent,
    BoardDeletePromptComponent,
    BoardMembersComponent,
    BoardItemListComponent,
    ItemCommentsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full', canActivate: [AuthActivatorService], data: {requiredLogin: true}},
      {path: 'register', component: RegisterComponent, canActivate: [AuthActivatorService]},
      {path: 'login', component: LoginComponent, canActivate: [AuthActivatorService]},
      {path: 'search', component: SearchComponent, canActivate: [AuthActivatorService], data: {requiredLogin: true}},
      {path: 'board/:boardId', component: BoardComponent, canActivate: [AuthActivatorService], data: {requiredLogin: true}},
      {path: '**', redirectTo: ''}
    ]),
    FormsModule
  ],
  providers: [httpInterceptorProviders, AuthActivatorService],
  bootstrap: [AppComponent]
})
export class AppModule {
}
