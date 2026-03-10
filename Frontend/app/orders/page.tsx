"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api from "@/lib/api";

interface OrderDto {
  id: number;
  status: "Pending" | "Processing" | "Shipped" | "Delivered" | "Cancelled";
  totalAmount: number;
  shippingAddress: string;
  createdAt: string;
  orderType?: string;
  shippingCost?: number;
}

export default function OrdersPage() {
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    api.get<OrderDto[]>("/orders")
      .then((response) => setOrders(response.data))
      .catch(() => setError("Nu s-au putut incarca comenzile."))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return <div className="flex justify-center p-8">Incarcare...</div>;
  }

  if (error) {
    return <div className="text-red-500 p-4">{error}</div>;
  }

  return (
    <div className="max-w-6xl mx-auto p-6">
      <div className="flex flex-wrap justify-between items-center gap-3 mb-6">
        <h1 className="text-3xl font-bold">Comenzile mele</h1>
        <div className="flex gap-2">
          <Link href="/orders/new" className="bg-emerald-600 hover:bg-emerald-700 text-white px-3 py-2 rounded-lg">
            Comanda noua
          </Link>
          <Link href="/orders/custom" className="bg-cyan-600 hover:bg-cyan-700 text-white px-3 py-2 rounded-lg">
            Comanda custom
          </Link>
          <Link href="/checkout" className="bg-amber-600 hover:bg-amber-700 text-white px-3 py-2 rounded-lg">
            Checkout
          </Link>
        </div>
      </div>
      <div className="overflow-x-auto bg-white rounded-xl shadow-md">
        <table className="w-full border-collapse">
          <thead>
            <tr>
              <th className="bg-gray-100 px-4 py-2 text-left">ID</th>
              <th className="bg-gray-100 px-4 py-2 text-left">Tip</th>
              <th className="bg-gray-100 px-4 py-2 text-left">Status</th>
              <th className="bg-gray-100 px-4 py-2 text-left">Total</th>
              <th className="bg-gray-100 px-4 py-2 text-left">Livrare</th>
              <th className="bg-gray-100 px-4 py-2 text-left">Data</th>
              <th className="bg-gray-100 px-4 py-2 text-left">Actiuni</th>
            </tr>
          </thead>
          <tbody>
            {orders.map((order) => (
              <tr key={order.id}>
                <td className="px-4 py-2 border-b border-gray-200">#{order.id}</td>
                <td className="px-4 py-2 border-b border-gray-200">{order.orderType ?? "Standard"}</td>
                <td className="px-4 py-2 border-b border-gray-200">{order.status}</td>
                <td className="px-4 py-2 border-b border-gray-200">{order.totalAmount.toFixed(2)} RON</td>
                <td className="px-4 py-2 border-b border-gray-200">{(order.shippingCost ?? 15).toFixed(2)} RON</td>
                <td className="px-4 py-2 border-b border-gray-200">{new Date(order.createdAt).toLocaleString()}</td>
                <td className="px-4 py-2 border-b border-gray-200">
                  <Link href={`/orders/${order.id}`} className="bg-blue-600 hover:bg-blue-700 text-white px-3 py-1 rounded-lg">
                    View Details
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
