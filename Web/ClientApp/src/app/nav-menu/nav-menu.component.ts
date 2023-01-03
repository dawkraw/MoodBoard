import {Component, OnInit} from '@angular/core';
import {StorageService} from "../services/storage.service";
import {AuthService} from "../services/auth.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  isLoggedIn = false;
  searchInput = '';

  constructor(private storageService: StorageService, private authService: AuthService, private router: Router) {}

  get username() {
    return this.storageService.getUser().userName;
  }

  ngOnInit(): void {
    if (this.storageService.isLoggedIn()) {
      this.isLoggedIn = true;
    }
  }

  searchNavigate() {
    if (this.searchInput.length > 0) {
      this.router.navigate(
        ['/search'],
        {queryParams: {query: this.searchInput}}
      );
    }
  }

  logout(): void {
    this.authService.logout().subscribe(
      {
        next: () => {
          this.storageService.clearUser();
          window.location.reload();
        }
      }
    )
  }
}
