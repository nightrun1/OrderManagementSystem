"use client";

import Link from "next/link";
import { FormEvent, useEffect, useMemo, useState } from "react";
import { AxiosError } from "axios";
import api from "@/lib/api";

type Provider = "Stripe" | "PayPal";

interface OrderItemDto {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}

interface OrderDto {
  id: number;
  status: string;
  totalAmount: number;
  shippingAddress: string;
  createdAt: string;
  items: OrderItemDto[];
}

interface PaymentApiResponse {
  success: boolean;
  transactionId: string;
  amount: number;
  currency: string;
  errorMessage: string;
  receiptText: string;
  providerName: string;
}

interface ApiErrorBody {
  message?: string;
}

export default function CheckoutPage() {
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [selectedOrderId, setSelectedOrderId] = useState<string>("");
  const [provider, setProvider] = useState<Provider>("Stripe");
  const [cardToken, setCardToken] = useState("");
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [paymentResult, setPaymentResult] = useState<PaymentApiResponse | null>(null);

  useEffect(() => {
    api
      .get<OrderDto[]>("/orders")
      .then((response) => {
        setOrders(response.data);
        if (response.data.length > 0) {
          setSelectedOrderId(String(response.data[0].id));
        }
      })
      .catch(() => setError("Nu s-au putut incarca comenzile pentru checkout."))
      .finally(() => setLoading(false));
  }, []);

  const selectedOrder = useMemo(
    () => orders.find((order) => String(order.id) === selectedOrderId) ?? null,
    [orders, selectedOrderId]
  );

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);
    setPaymentResult(null);

    if (!selectedOrder) {
      setError("Selecteaza o comanda pentru plata.");
      return;
    }

    if (!cardToken.trim()) {
      setError("Introdu un card token simulat.");
      return;
    }

    setSubmitting(true);

    try {
      const response = await api.post<PaymentApiResponse>(
        "/payments",
        {
          orderId: selectedOrder.id,
          cardToken: cardToken.trim(),
        },
        {
          headers: {
            "X-Payment-Provider": provider,
          },
        }
      );

      setPaymentResult(response.data);
    } catch (requestError: unknown) {
      const axiosError = requestError as AxiosError<ApiErrorBody>;
      const message = axiosError.response?.data?.message ?? "Plata a esuat.";
      setError(message);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <div className="p-8 text-center">Incarcare checkout...</div>;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-amber-50 to-orange-50 py-10 px-4">
      <div className="max-w-5xl mx-auto space-y-6">
        <header className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h1 className="text-3xl font-bold text-slate-900">Checkout</h1>
            <p className="text-slate-600 mt-1">Abstract Factory: Stripe sau PayPal pentru procesare si receipt.</p>
          </div>
          <div className="flex gap-2">
            <Link
              href="/orders"
              className="px-4 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-100 transition-colors"
            >
              Comenzile mele
            </Link>
            <Link
              href="/orders/new"
              className="px-4 py-2 rounded-lg bg-slate-900 text-white hover:bg-slate-700 transition-colors"
            >
              Comanda noua
            </Link>
          </div>
        </header>

        <form onSubmit={handleSubmit} className="grid lg:grid-cols-[1.3fr_1fr] gap-6">
          <section className="bg-white rounded-2xl shadow-xl border border-slate-100 p-6 space-y-4">
            <h2 className="text-xl font-semibold text-slate-900">Rezumat comanda</h2>

            {orders.length === 0 && (
              <div className="rounded-xl border border-slate-200 bg-slate-50 p-4 text-slate-700">
                Nu exista comenzi disponibile. Creeaza mai intai o comanda.
              </div>
            )}

            {orders.length > 0 && (
              <>
                <div>
                  <label htmlFor="orderId" className="block text-sm font-medium text-slate-700 mb-1">
                    Comanda
                  </label>
                  <select
                    id="orderId"
                    value={selectedOrderId}
                    onChange={(event) => setSelectedOrderId(event.target.value)}
                    className="w-full rounded-xl border border-slate-300 p-2.5"
                  >
                    {orders.map((order) => (
                      <option key={order.id} value={order.id}>
                        #{order.id} - {order.totalAmount.toFixed(2)} RON - {order.status}
                      </option>
                    ))}
                  </select>
                </div>

                {selectedOrder && (
                  <div className="rounded-xl border border-slate-200 bg-slate-50 p-4 space-y-2">
                    <p className="text-sm text-slate-700">Adresa: {selectedOrder.shippingAddress}</p>
                    <div className="space-y-1 text-sm text-slate-700">
                      {selectedOrder.items.map((item) => (
                        <div key={`${item.productId}-${item.productName}`} className="flex justify-between gap-3">
                          <span>
                            {item.productName} x {item.quantity}
                          </span>
                          <span>{(item.unitPrice * item.quantity).toFixed(2)} RON</span>
                        </div>
                      ))}
                    </div>
                    <p className="font-semibold text-slate-900 pt-2 border-t border-slate-200">
                      Total: {selectedOrder.totalAmount.toFixed(2)} RON
                    </p>
                  </div>
                )}
              </>
            )}
          </section>

          <section className="bg-white rounded-2xl shadow-xl border border-slate-100 p-6 space-y-4">
            <h2 className="text-xl font-semibold text-slate-900">Plata</h2>

            <div className="space-y-2">
              <p className="text-sm font-medium text-slate-700">Provider</p>
              <div className="grid grid-cols-2 gap-2">
                {(["Stripe", "PayPal"] as const).map((providerOption) => (
                  <label
                    key={providerOption}
                    className={`rounded-lg border px-3 py-2 cursor-pointer text-center ${
                      provider === providerOption
                        ? "border-amber-600 bg-amber-100 text-amber-900"
                        : "border-slate-300 hover:border-slate-500"
                    }`}
                  >
                    <input
                      type="radio"
                      name="provider"
                      value={providerOption}
                      checked={provider === providerOption}
                      onChange={() => setProvider(providerOption)}
                      className="sr-only"
                    />
                    {providerOption}
                  </label>
                ))}
              </div>
            </div>

            <div>
              <label htmlFor="cardToken" className="block text-sm font-medium text-slate-700 mb-1">
                Card token (simulat)
              </label>
              <input
                id="cardToken"
                value={cardToken}
                onChange={(event) => setCardToken(event.target.value)}
                placeholder="ex: card_ok_1234 (foloseste 'fail' pentru simulare eroare)"
                className="w-full rounded-xl border border-slate-300 p-2.5"
              />
            </div>

            {error && <p className="text-rose-600 text-sm font-medium">{error}</p>}

            <button
              type="submit"
              disabled={submitting || orders.length === 0}
              className="w-full rounded-xl py-3 bg-gradient-to-r from-amber-500 to-orange-600 text-white font-semibold hover:opacity-90 disabled:opacity-50"
            >
              {submitting ? "Se proceseaza plata..." : "Plateste comanda"}
            </button>

            <p className="text-xs text-slate-500">
              Daca ai setat `PaymentProvider` in backend, acesta devine providerul implicit.
            </p>
          </section>
        </form>

        {paymentResult && (
          <section
            className={`rounded-2xl p-5 border ${
              paymentResult.success
                ? "bg-emerald-50 border-emerald-200 text-emerald-900"
                : "bg-rose-50 border-rose-200 text-rose-900"
            }`}
          >
            <h3 className="text-xl font-semibold mb-2">
              {paymentResult.success ? "Plata reusita" : "Plata esuata"}
            </h3>
            <p>Provider folosit: {paymentResult.providerName}</p>
            <p>Transaction ID: {paymentResult.transactionId || "n/a"}</p>
            <p>
              Suma: {paymentResult.amount.toFixed(2)} {paymentResult.currency}
            </p>
            {!paymentResult.success && paymentResult.errorMessage && <p>Eroare: {paymentResult.errorMessage}</p>}
            {paymentResult.receiptText && (
              <pre className="mt-3 whitespace-pre-wrap rounded-xl bg-white/70 p-3 text-sm border border-current/20">
                {paymentResult.receiptText}
              </pre>
            )}
          </section>
        )}
      </div>
    </div>
  );
}
