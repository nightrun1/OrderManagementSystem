"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { AxiosError } from "axios";
import api from "@/lib/api";

interface TemplateItemDto {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}

interface OrderTemplateDto {
  id: number;
  name: string;
  createdByUserId: number;
  shippingAddress: string;
  createdAt: string;
  itemCount: number;
  items: TemplateItemDto[];
}

interface ClonedOrderDto {
  id: number;
  status: string;
  totalAmount: number;
  shippingAddress: string;
  createdAt: string;
}

interface ErrorMessageDto {
  message?: string;
}

export default function OrderTemplatesPage() {
  const [templates, setTemplates] = useState<OrderTemplateDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [actionMessage, setActionMessage] = useState<string | null>(null);
  const [busyTemplateId, setBusyTemplateId] = useState<number | null>(null);

  const loadTemplates = async () => {
    setError(null);

    try {
      const response = await api.get<OrderTemplateDto[]>("/orders/templates");
      setTemplates(response.data);
    } catch {
      setError("Nu s-au putut incarca template-urile.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadTemplates();
  }, []);

  const handleClone = async (templateId: number) => {
    setBusyTemplateId(templateId);
    setActionMessage(null);
    setError(null);

    try {
      const response = await api.post<ClonedOrderDto>(`/orders/templates/${templateId}/clone`);
      setActionMessage(`Comanda #${response.data.id} a fost creata din template.`);
    } catch (requestError: unknown) {
      if (requestError instanceof AxiosError) {
        const responseData = requestError.response?.data as ErrorMessageDto | undefined;
        setError(responseData?.message ?? "Nu s-a putut clona template-ul.");
      } else {
        setError("Nu s-a putut clona template-ul.");
      }
    } finally {
      setBusyTemplateId(null);
    }
  };

  const handleDelete = async (templateId: number) => {
    setBusyTemplateId(templateId);
    setActionMessage(null);
    setError(null);

    try {
      await api.delete(`/orders/templates/${templateId}`);
      setTemplates((previous) => previous.filter((template) => template.id !== templateId));
      setActionMessage("Template-ul a fost sters.");
    } catch (requestError: unknown) {
      if (requestError instanceof AxiosError) {
        const responseData = requestError.response?.data as ErrorMessageDto | undefined;
        setError(responseData?.message ?? "Nu s-a putut sterge template-ul.");
      } else {
        setError("Nu s-a putut sterge template-ul.");
      }
    } finally {
      setBusyTemplateId(null);
    }
  };

  if (loading) {
    return <div className="p-8 text-center">Incarcare template-uri...</div>;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-emerald-50 py-10 px-4">
      <div className="max-w-5xl mx-auto space-y-6">
        <header className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h1 className="text-3xl font-bold text-slate-900">Comenzile mele salvate</h1>
            <p className="text-slate-600">Prototype pattern pentru recomandari rapide din template-uri.</p>
          </div>
          <div className="flex gap-2">
            <Link href="/orders" className="px-4 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-100">
              Inapoi la comenzi
            </Link>
            <Link href="/orders/custom" className="px-4 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700">
              Comanda custom
            </Link>
          </div>
        </header>

        {error && <p className="text-rose-600 font-medium">{error}</p>}
        {actionMessage && <p className="text-emerald-700 font-medium">{actionMessage}</p>}

        {templates.length === 0 ? (
          <div className="bg-white border border-slate-100 rounded-2xl shadow-md p-8 text-center text-slate-600">
            Nu ai template-uri salvate inca.
          </div>
        ) : (
          <div className="space-y-4">
            {templates.map((template) => (
              <article key={template.id} className="bg-white border border-slate-100 rounded-2xl shadow-md p-5 space-y-4">
                <div className="flex flex-wrap items-start justify-between gap-3">
                  <div>
                    <h2 className="text-xl font-semibold text-slate-900">{template.name}</h2>
                    <p className="text-sm text-slate-600">Creat la: {new Date(template.createdAt).toLocaleString()}</p>
                    <p className="text-sm text-slate-600">Adresa: {template.shippingAddress}</p>
                    <p className="text-sm text-slate-600">Numar produse: {template.itemCount}</p>
                  </div>

                  <div className="flex gap-2">
                    <button
                      onClick={() => void handleClone(template.id)}
                      disabled={busyTemplateId === template.id}
                      className="px-3 py-2 rounded-lg bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-50"
                    >
                      Recomanda
                    </button>
                    <button
                      onClick={() => void handleDelete(template.id)}
                      disabled={busyTemplateId === template.id}
                      className="px-3 py-2 rounded-lg border border-rose-300 text-rose-700 hover:bg-rose-50 disabled:opacity-50"
                    >
                      Sterge template
                    </button>
                  </div>
                </div>

                <div className="rounded-xl border border-slate-200 p-3">
                  <h3 className="font-medium text-slate-800 mb-2">Produse in template</h3>
                  <div className="space-y-1 text-sm text-slate-700">
                    {template.items.map((item) => (
                      <p key={`${template.id}-${item.productId}-${item.productName}`}>
                        {item.productName} x {item.quantity} ({item.unitPrice.toFixed(2)} RON)
                      </p>
                    ))}
                  </div>
                </div>
              </article>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
