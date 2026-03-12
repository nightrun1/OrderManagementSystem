"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { AxiosError } from "axios";
import api from "@/lib/api";

type Provider = "Stripe" | "PayPal";

interface CartItemDto {
  id: number;
  name: string;
  price: number;
  quantity: number;
  isBundle: boolean;
  depth: number;
}

interface CartResponse {
  items: CartItemDto[];
  total: number;
  itemCount: number;
}

interface ShippingQuote {
  provider: string;
  price: number;
  estimatedDays: number;
  serviceType: string;
}

interface PlaceOrderResult {
  success: boolean;
  orderId?: number;
  errorMessage?: string;
  errorStep?: string;
  totalCharged: number;
  trackingNumber?: string;
  receiptText: string;
}

interface ProductDto {
  id: number;
  name: string;
  price: number;
}

interface ApiErrorBody {
  message?: string;
  errorStep?: string;
  errorMessage?: string;
}

const STEPS = ["StockCheck", "OrderCreate", "Payment", "Shipping", "Email", "Done"];
const STEP_LABELS: Record<string, string> = {
  StockCheck: "Verificare stoc",
  OrderCreate: "Creare comanda",
  Payment: "Procesare plata",
  Shipping: "Expediere creata",
  Email: "Email trimis",
  Done: "Finalizat",
};

export default function CheckoutPage() {
  const [cart, setCart] = useState<CartResponse>({ items: [], total: 0, itemCount: 0 });
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [shippingAddress, setShippingAddress] = useState("");
  const [deliveryOption, setDeliveryOption] = useState("FanCourier");
  const [provider, setProvider] = useState<Provider>("Stripe");
  const [cardToken, setCardToken] = useState("");
  const [discountCode, setDiscountCode] = useState("");
  const [shippingQuotes, setShippingQuotes] = useState<ShippingQuote[]>([]);

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [result, setResult] = useState<PlaceOrderResult | null>(null);
  const [failedStep, setFailedStep] = useState<string | null>(null);

  useEffect(() => {
    Promise.all([
      api.get<CartResponse>("/cart"),
      api.get<ProductDto[]>("/products"),
    ])
      .then(([cartRes, prodRes]) => {
        setCart(cartRes.data);
        setProducts(prodRes.data);
      })
      .catch(() => setError("Nu s-a putut incarca cosul."))
      .finally(() => setLoading(false));
  }, []);

  const fetchQuotes = async () => {
    if (!shippingAddress) return;
    try {
      const res = await api.get<ShippingQuote[]>("/shipping/quotes", {
        params: { fromCity: "Depozit", toCity: shippingAddress.split(",")[0] || shippingAddress, weightKg: 2 },
      });
      setShippingQuotes(res.data);
    } catch {
      /* ignore */
    }
  };

  const handleSubmit = async () => {
    setError(null);
    setResult(null);
    setFailedStep(null);

    if (cart.items.length === 0) {
      setError("Cosul este gol. Adauga produse inainte de checkout.");
      return;
    }
    if (!shippingAddress.trim()) {
      setError("Introdu adresa de livrare.");
      return;
    }
    if (!cardToken.trim()) {
      setError("Introdu un card token simulat.");
      return;
    }

    // Build items from cart — use only leaf items (non-bundles with depth >= 0)
    const items = cart.items
      .filter((i) => !i.isBundle)
      .map((i) => {
        const prod = products.find((p) => p.id === i.id);
        return { productId: i.id, quantity: i.quantity, unitPrice: prod?.price ?? i.price };
      });

    if (items.length === 0) {
      setError("Nu exista produse valide in cos.");
      return;
    }

    setSubmitting(true);

    try {
      const res = await api.post<PlaceOrderResult>(
        "/checkout",
        {
          items,
          shippingAddress: shippingAddress.trim(),
          paymentToken: cardToken.trim(),
          paymentProvider: provider,
          discountCode: discountCode || null,
          deliveryOption,
        },
        {
          headers: { "X-Payment-Provider": provider },
        }
      );
      setResult(res.data);
    } catch (err: unknown) {
      const axiosErr = err as AxiosError<ApiErrorBody>;
      const body = axiosErr.response?.data;
      setFailedStep(body?.errorStep ?? null);
      setError(body?.errorMessage ?? body?.message ?? "Checkout a esuat.");
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <div className="p-8 text-center">Incarcare checkout...</div>;

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-amber-50 to-orange-50 py-10 px-4">
      <div className="max-w-5xl mx-auto space-y-6">
        <header className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h1 className="text-3xl font-bold text-slate-900">Checkout (Facade Pattern)</h1>
            <p className="text-slate-600 mt-1">Facade: un singur endpoint care orchestreaza stoc, comanda, plata, expediere si email.</p>
          </div>
          <div className="flex gap-2">
            <Link href="/cart" className="px-4 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-100">Cos</Link>
            <Link href="/orders" className="px-4 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-100">Comenzi</Link>
            <Link href="/shipping" className="px-4 py-2 rounded-lg bg-slate-900 text-white hover:bg-slate-700">Livrare</Link>
          </div>
        </header>

        <div className="grid lg:grid-cols-[1.3fr_1fr] gap-6">
          {/* Left: Cart summary + address + delivery */}
          <div className="space-y-6">
            {/* 1. Cart summary */}
            <section className="bg-white rounded-2xl shadow-xl border border-slate-100 p-6">
              <h2 className="text-xl font-semibold text-slate-900 mb-3">1. Rezumat cos</h2>
              {cart.items.length === 0 ? (
                <p className="text-slate-500">Cosul este gol. <Link href="/cart" className="text-blue-600 underline">Adauga produse</Link></p>
              ) : (
                <div className="space-y-2">
                  {cart.items.map((item, idx) => (
                    <div key={idx} className="flex justify-between text-sm" style={{ paddingLeft: `${item.depth * 16}px` }}>
                      <span>{item.isBundle ? "📦 " : item.depth > 0 ? "└── " : ""}{item.name} {!item.isBundle && `x${item.quantity}`}</span>
                      <span>{item.price.toFixed(2)} RON</span>
                    </div>
                  ))}
                  <p className="font-semibold pt-2 border-t text-slate-900">Total: {cart.total.toFixed(2)} RON</p>
                </div>
              )}
            </section>

            {/* 2. Adresa livrare */}
            <section className="bg-white rounded-2xl shadow-xl border border-slate-100 p-6">
              <h2 className="text-xl font-semibold text-slate-900 mb-3">2. Adresa livrare</h2>
              <input
                type="text" placeholder="ex: Cluj-Napoca, str. Exemplu 10"
                value={shippingAddress}
                onChange={(e) => setShippingAddress(e.target.value)}
                onBlur={fetchQuotes}
                className="w-full rounded-xl border border-slate-300 p-2.5"
              />
            </section>

            {/* 3. Optiune livrare */}
            <section className="bg-white rounded-2xl shadow-xl border border-slate-100 p-6">
              <h2 className="text-xl font-semibold text-slate-900 mb-3">3. Optiune livrare</h2>
              <div className="grid grid-cols-2 gap-2 mb-3">
                {["FanCourier", "DPD"].map((opt) => (
                  <label
                    key={opt}
                    className={`rounded-lg border px-3 py-2 cursor-pointer text-center ${
                      deliveryOption === opt ? "border-emerald-600 bg-emerald-100 text-emerald-900" : "border-slate-300 hover:border-slate-500"
                    }`}
                  >
                    <input type="radio" name="delivery" value={opt} checked={deliveryOption === opt} onChange={() => setDeliveryOption(opt)} className="sr-only" />
                    {opt}
                  </label>
                ))}
              </div>
              {shippingQuotes.length > 0 && (
                <div className="text-sm text-slate-600 space-y-1">
                  {shippingQuotes.map((q) => (
                    <div key={q.provider} className={`flex justify-between ${q.provider === deliveryOption ? "font-semibold text-slate-900" : ""}`}>
                      <span>{q.provider} — {q.serviceType}</span>
                      <span>{q.price.toFixed(2)} RON, ~{q.estimatedDays} zile</span>
                    </div>
                  ))}
                </div>
              )}
            </section>
          </div>

          {/* Right: Payment + voucher + submit */}
          <div className="space-y-6">
            {/* 4. Metoda plata */}
            <section className="bg-white rounded-2xl shadow-xl border border-slate-100 p-6 space-y-4">
              <h2 className="text-xl font-semibold text-slate-900">4. Metoda plata</h2>
              <div className="grid grid-cols-2 gap-2">
                {(["Stripe", "PayPal"] as const).map((provOpt) => (
                  <label
                    key={provOpt}
                    className={`rounded-lg border px-3 py-2 cursor-pointer text-center ${
                      provider === provOpt ? "border-amber-600 bg-amber-100 text-amber-900" : "border-slate-300 hover:border-slate-500"
                    }`}
                  >
                    <input type="radio" name="provider" value={provOpt} checked={provider === provOpt} onChange={() => setProvider(provOpt)} className="sr-only" />
                    {provOpt}
                  </label>
                ))}
              </div>
              <input
                type="text" placeholder="Card token simulat (ex: card_ok_1234)"
                value={cardToken}
                onChange={(e) => setCardToken(e.target.value)}
                className="w-full rounded-xl border border-slate-300 p-2.5"
              />
            </section>

            {/* 5. Cod voucher */}
            <section className="bg-white rounded-2xl shadow-xl border border-slate-100 p-6">
              <h2 className="text-xl font-semibold text-slate-900 mb-3">5. Cod voucher (optional)</h2>
              <input
                type="text" placeholder="ex: DISCOUNT10"
                value={discountCode}
                onChange={(e) => setDiscountCode(e.target.value)}
                className="w-full rounded-xl border border-slate-300 p-2.5"
              />
            </section>

            {/* Submit */}
            <div>
              {error && <p className="text-rose-600 text-sm font-medium mb-2">{error}</p>}
              <button
                onClick={handleSubmit}
                disabled={submitting || cart.items.length === 0}
                className="w-full rounded-xl py-3 bg-gradient-to-r from-amber-500 to-orange-600 text-white font-semibold hover:opacity-90 disabled:opacity-50"
              >
                {submitting ? "Se proceseaza..." : "Plaseaza comanda"}
              </button>
            </div>
          </div>
        </div>

        {/* Progress stepper */}
        {(result || failedStep) && (
          <section className={`rounded-2xl p-5 border ${
            result?.success ? "bg-emerald-50 border-emerald-200" : "bg-rose-50 border-rose-200"
          }`}>
            <h3 className="text-xl font-semibold mb-4">
              {result?.success ? "Comanda plasata cu succes!" : "Comanda a esuat"}
            </h3>

            {/* Stepper */}
            <div className="flex flex-wrap gap-2 mb-4">
              {STEPS.map((step) => {
                const isFailed = failedStep === step;
                const failIdx = failedStep ? STEPS.indexOf(failedStep) : -1;
                const stepIdx = STEPS.indexOf(step);
                const isPassed = result?.success || (failIdx >= 0 ? stepIdx < failIdx : false);

                return (
                  <div
                    key={step}
                    className={`px-3 py-1 rounded-full text-sm font-medium ${
                      isFailed ? "bg-red-200 text-red-800" : isPassed ? "bg-emerald-200 text-emerald-800" : "bg-gray-200 text-gray-600"
                    }`}
                  >
                    {isFailed ? "❌" : isPassed ? "✅" : "⬜"} {STEP_LABELS[step] ?? step}
                  </div>
                );
              })}
            </div>

            {result?.success && (
              <div className="space-y-1 text-sm">
                <p>Comanda ID: <span className="font-semibold">#{result.orderId}</span></p>
                <p>Total: <span className="font-semibold">{result.totalCharged.toFixed(2)} RON</span></p>
                <p>Tracking: <span className="font-semibold">{result.trackingNumber}</span></p>
                {result.receiptText && (
                  <pre className="mt-3 whitespace-pre-wrap rounded-xl bg-white/70 p-3 text-sm border">{result.receiptText}</pre>
                )}
                <Link href={`/orders/${result.orderId}`} className="inline-block mt-3 bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-2 rounded-lg">
                  Vezi comanda
                </Link>
              </div>
            )}

            {!result?.success && error && (
              <p className="text-rose-700">{error}</p>
            )}
          </section>
        )}
      </div>
    </div>
  );
}
