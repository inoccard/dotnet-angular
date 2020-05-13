import { EventService } from './../services/event/event.service';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-events',
  templateUrl: './events.component.html',
  styleUrls: ['./events.component.css']
})
export class EventsComponent implements OnInit {
  eventsFiltered: any = [];
  events: any = [];
  showImg = false;
  // tslint:disable-next-line: variable-name
  _filterList: string;
  modalRef: BsModalRef;
  registerForm: FormGroup;
  constructor(private eventService: EventService, private modalService: BsModalService) { }
  ngOnInit() {
    this.validation();
    this.getEvents();
  }
  get filterList(): string {
    return this._filterList;
  }
  set filterList(value: string) {
    this._filterList = value;
    this.eventsFiltered = this._filterList ? this.filterEvent(this._filterList) : this.events;
  }
  // Abrir modal
  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }
  filterEvent(filterBy: string): any {
    filterBy = filterBy.toLocaleLowerCase();
    return this.events.filter(
      (      event: { theme: string; }) => event.theme.toLocaleLowerCase().indexOf(filterBy) !== -1
    );
  }
  // mostrar | ocultar imagem
  alterImg() {
    this.showImg = !this.showImg;
  }
  // Obter eventos
  getEvents() {
    this.eventService.getEvents().subscribe(
      response => {
        this.events = response;
        this.eventsFiltered = this.events;
      }, error => {
        console.log(error);
      }
    );
  }

  validation() {
    this.registerForm = new FormGroup({
      theme: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]),
      local: new FormControl('', Validators.required),
      eventDate: new FormControl(),
      personQtd: new FormControl('', [Validators.required, Validators.max(120000)]),
      imageURL: new FormControl('', Validators.required),
      contactPhone: new FormControl('', Validators.required),
      contactEmail: new FormControl('', [Validators.required, Validators.email])
    });
  }
  // Salvar alterações
  saveEditions() {

  }
}
