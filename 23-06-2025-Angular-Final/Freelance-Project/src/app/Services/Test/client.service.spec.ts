import { TestBed } from '@angular/core/testing';
import { ClientService } from '../client.service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

describe('ClientService', () => {
  let service: ClientService;
  let httpSpy: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    httpSpy = jasmine.createSpyObj('HttpClient', ['get', 'post', 'put', 'delete']);
    TestBed.configureTestingModule({
      providers: [
        ClientService,
        { provide: HttpClient, useValue: httpSpy }
      ]
    });
    service = TestBed.inject(ClientService);
  });

  it('should get all clients', () => {
    httpSpy.get.and.returnValue(of({}));
    service.getAllClients().subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should get client by id', () => {
    httpSpy.get.and.returnValue(of({}));
    service.getClientById('id').subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should create client', () => {
    httpSpy.post.and.returnValue(of({}));
    service.createClient({} as any).subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
  });

  it('should update client', () => {
    httpSpy.put.and.returnValue(of({}));
    service.updateClient('id', {} as any).subscribe();
    expect(httpSpy.put).toHaveBeenCalled();
  });

  it('should delete client', () => {
    httpSpy.delete.and.returnValue(of({}));
    service.deleteClient('id').subscribe();
    expect(httpSpy.delete).toHaveBeenCalled();
  });
});
