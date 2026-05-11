export interface RoomDto {
  id: number;
  hotelId: number;
  hotelName: string;
  roomNumber: string;
  type: string;
  capacity: number;
  priceOffPeak: number;
  pricePeak: number;
  status: string;
  description: string;
  floorNumber: number;
}
