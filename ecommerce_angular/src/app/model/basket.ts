import { User } from "./user";
import { Product } from './product'

export interface Basket {
    id: number,
    user?: any, 
    product?: any,
    quantity: number,
    orderNumber: number
}