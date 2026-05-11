import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import type { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import type { RoomDto } from '../models/room.models';

@Injectable({ providedIn: 'root' })
export class RoomsApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiRoot}/Rooms`;

  getById(id: number): Observable<RoomDto> {
    return this.http.get<RoomDto>(`${this.base}/${id}`);
  }

  searchAvailable(params: {
    hotelId: number;
    checkIn: string;
    checkOut: string;
    minCapacity?: number;
  }): Observable<RoomDto[]> {
    let hp = new HttpParams()
      .set('hotelId', String(params.hotelId))
      .set('checkIn', params.checkIn)
      .set('checkOut', params.checkOut);
    if (params.minCapacity != null) {
      hp = hp.set('minCapacity', String(params.minCapacity));
    }
    return this.http.get<RoomDto[]>(`${this.base}/available`, { params: hp });
  }
}
