"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import api from "@/lib/api";

interface OrderItemDto {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}

interface OrderDto {
  id: number;
  status: "Pending" | "Processing" | "Shipped" | "Delivered" | "Cancelled";
  totalAmount: number;
  shippingAddress: string;
  createdAt: string;
  items: OrderItemDto[];
}

export default function OrderDetailsPage() {
  const params = useParams<{ id: string }>();
  const [order, setOrder] = useState<OrderDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const orderId = params.id;
    api.get<OrderDto>(`/orders/${orderId}`)
      .then((response) => setOrder(response.data))
      .catch(() => setError("Nu s-au putut incarca detaliile comenzii."))
      .finally(() => setLoading(false));
  }, [params.id]);

  if (loading) {
    return <div className="flex justify-center p-8">Incarcare...</div>;
  }

  if (error) {
    return <div className="text-red-500 p-4">{error}</div>;
  }

  if (!order) {
    return <div className="p-4">Comanda nu a fost gasita.</div>;
  }

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-4">
      <h1 className="text-3xl font-bold">Comanda #{order.id}</h1>
      <div className="bg-white rounded-xl shadow-md p-6 space-y-2">
        <p><strong>Status:</strong> {order.status}</p>
        <p><strong>Total:</strong> {order.totalAmount.toFixed(2)} RON</p>
        <p><strong>Adresa:</strong> {order.shippingAddress}</p>
        <p><strong>Data:</strong> {new Date(order.createdAt).toLocaleString()}</p>
      </div>

      <div className="bg-white rounded-xl shadow-md p-6">
        <h2 className="text-xl font-semibold mb-4">Produse</h2>
        <div className="space-y-2">
          {order.items.map((item) => (
            <div key={item.productId} className="flex justify-between border-b border-gray-200 pb-2">
              <span>{item.productName} x {item.quantity}</span>
              <span>{item.unitPrice.toFixed(2)} RON</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
