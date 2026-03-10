"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { FormEvent, useEffect, useState } from "react";
import { AxiosError } from "axios";
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
  orderType?: string;
  shippingCost?: number;
}

interface OrderTemplateDto {
  id: number;
  name: string;
}

interface ErrorMessageDto {
  message?: string;
}

export default function OrderDetailsPage() {
  const params = useParams<{ id: string }>();
  const [order, setOrder] = useState<OrderDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showTemplateForm, setShowTemplateForm] = useState(false);
  const [templateName, setTemplateName] = useState("");
  const [savingTemplate, setSavingTemplate] = useState(false);
  const [templateError, setTemplateError] = useState<string | null>(null);
  const [templateSuccess, setTemplateSuccess] = useState<string | null>(null);

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

  const handleSaveAsTemplate = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setTemplateError(null);
    setTemplateSuccess(null);

    if (!templateName.trim()) {
      setTemplateError("Numele template-ului este obligatoriu.");
      return;
    }

    setSavingTemplate(true);

    try {
      const response = await api.post<OrderTemplateDto>(`/orders/${params.id}/save-as-template`, {
        name: templateName.trim(),
      });

      setTemplateSuccess(`Template salvat: ${response.data.name}.`);
      setTemplateName("");
      setShowTemplateForm(false);
    } catch (requestError: unknown) {
      if (requestError instanceof AxiosError) {
        const responseData = requestError.response?.data as ErrorMessageDto | undefined;
        setTemplateError(responseData?.message ?? "Salvarea template-ului a esuat.");
      } else {
        setTemplateError("Salvarea template-ului a esuat.");
      }
    } finally {
      setSavingTemplate(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-4">
      <div className="flex flex-wrap justify-between items-center gap-3">
        <h1 className="text-3xl font-bold">Comanda #{order.id}</h1>
        <div className="flex gap-2">
          <Link href="/orders/templates" className="px-3 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-100">
            Template-uri
          </Link>
          <button
            onClick={() => {
              setShowTemplateForm((previous) => !previous);
              setTemplateError(null);
              setTemplateSuccess(null);
            }}
            className="px-3 py-2 rounded-lg bg-indigo-600 hover:bg-indigo-700 text-white"
          >
            {showTemplateForm ? "Anuleaza" : "Salveaza ca template"}
          </button>
        </div>
      </div>

      {showTemplateForm && (
        <form onSubmit={handleSaveAsTemplate} className="bg-indigo-50 border border-indigo-200 rounded-xl p-4 space-y-3">
          <label htmlFor="templateName" className="block text-sm font-medium text-slate-800">
            Nume template
          </label>
          <input
            id="templateName"
            value={templateName}
            onChange={(event) => setTemplateName(event.target.value)}
            className="w-full rounded-lg border border-slate-300 px-3 py-2"
            placeholder="Ex: Aprovizionare birou lunar"
          />
          <button
            type="submit"
            disabled={savingTemplate}
            className="px-4 py-2 rounded-lg bg-indigo-600 text-white hover:bg-indigo-700 disabled:opacity-50"
          >
            {savingTemplate ? "Se salveaza..." : "Salveaza template"}
          </button>
        </form>
      )}

      {templateError && <p className="text-rose-600 font-medium">{templateError}</p>}
      {templateSuccess && <p className="text-emerald-700 font-medium">{templateSuccess}</p>}

      <div className="bg-white rounded-xl shadow-md p-6 space-y-2">
        <p><strong>Status:</strong> {order.status}</p>
        <p><strong>Tip comanda:</strong> {order.orderType ?? "Standard"}</p>
        <p><strong>Total:</strong> {order.totalAmount.toFixed(2)} RON</p>
        <p><strong>Cost livrare:</strong> {(order.shippingCost ?? 15).toFixed(2)} RON</p>
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
