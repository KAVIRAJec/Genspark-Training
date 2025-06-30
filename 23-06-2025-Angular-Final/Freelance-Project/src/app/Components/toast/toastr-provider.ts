import { importProvidersFrom } from '@angular/core';
import { ToastrModule } from 'ngx-toastr';

export const provideToastr = () => importProvidersFrom(
  ToastrModule.forRoot({
    positionClass: 'toast-top-right',
    timeOut: 3000,
    closeButton: true,
    progressBar: true,
    preventDuplicates: true,
  })
);
