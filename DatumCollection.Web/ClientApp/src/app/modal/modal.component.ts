import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Control } from '../form/control';
import { SpiderScheduleService } from '../spider-schedule.service';
import { IService } from '../services/interface';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css'],
})
export class ModalComponent {

  title: string;

}

