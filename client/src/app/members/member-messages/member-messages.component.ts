import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { MessageService } from '../../services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, TimeagoModule]
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm;
  @Input() username?: string;
  messageContent = '';
  loading = false;

  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
    
  }

  sendMessage() {
    if (!this.username) return;
    this.loading = true;

    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm?.reset();
    }).finally(() => this.loading = false);
  }

}
