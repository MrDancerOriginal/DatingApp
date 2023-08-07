import { Component, OnInit } from '@angular/core';
import { Photo } from '../../models/photo';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

  photos: Photo[] = [];

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe({
      next: photos => this.photos = photos
    });
  }

  approve(id: number) {
    this.adminService.approvePhoto(id).subscribe({
      next: () => {
        const index = this.photos.findIndex(p => p.id === id);
        this.photos.splice(index, 1);
      }
    });
  }

  reject(id: number) {
    this.adminService.rejectPhoto(id).subscribe({
      next: () => {
        const index = this.photos.findIndex(p => p.id === id);
        this.photos.splice(index, 1);
      }
    });
  }

}
