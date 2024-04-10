import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Validators } from '@angular/forms';
import { InputTextareaModule } from 'primeng/inputtextarea';

@Component({
  selector: 'app-contact-us',
  standalone: true,
  imports: [
    ButtonModule,
    InputTextModule,
    ReactiveFormsModule,
    InputTextareaModule
  ],
  templateUrl: './contact-us.component.html',
  styleUrl: './contact-us.component.css'
})
export class ContactUsComponent implements OnInit {

  private formBuilder:FormBuilder = inject(FormBuilder);

  contactUsForm: FormGroup = new FormGroup([]);

  ngOnInit(): void {
    this.createForm();
  }

  createForm(): void {
    this.contactUsForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      company: [''],
      message: ['', [
        Validators.maxLength(500)
      ]],
    });
  }


  onSubmit(): void {
    console.log(this.contactUsForm.value);

    this.createForm();
  }

}
