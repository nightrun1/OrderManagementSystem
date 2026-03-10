"use client";

import Link from "next/link";
import { FormEvent, useEffect, useMemo, useState } from "react";
import { AxiosError } from "axios";
import api from "@/lib/api";

type DeliveryOption = "standard" | "express" | "pickup";
type PresetType = "quick" | "priority";

type Step = 1 | 2 | 3;

interface ProductDto {
  id: number;
  name: string;
  price: number;
  stock: number;
}

interface OrderItemInput {
  productId: string;
  quantity: number;
}

interface CustomOrderResultDto {
  orderId: number;
  status: string;
  finalTotal: number;
  shippingCost: number;
  discountAmount: number;
  deliveryOption: string;
  isPriority: boolean;
  shippingAddress: string;
  createdAt: string;
  customerNote: string | null;
}

interface ErrorMessageDto {
  message?: string;
}

const DELIVERY_BASE_COST: Record<DeliveryOption, number> = {
  standard: 15,
  express: 45,
  pickup: 0,
};

const STEP_LABELS: Array<{ id: Step; label: string }> = [
  { id: 1, label: "Produse" },
  { id: 2, label: "Livrare" },
  { id: 3, label: "Extras" },
];

function createEmptyItem(): OrderItemInput {
  return {
    productId: "",
    quantity: 1,
  };
}

function getUserIdFromToken(): number {
  if (typeof window === "undefined") {
    return 0;
  }

  const token = window.localStorage.getItem("token");
  if (!token) {
    return 0;
  }

  try {
    const payload = token.split(".")[1];
    if (!payload) {
      return 0;
    }

    const normalizedPayload = payload.replace(/-/g, "+").replace(/_/g, "/");
    const decoded = window.atob(normalizedPayload);
    const parsedPayload = JSON.parse(decoded) as Record<string, unknown>;

    const idValue =
      parsedPayload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ?? parsedPayload.sub;

    const userId = Number(idValue);
    return Number.isFinite(userId) ? userId : 0;
  } catch {
    return 0;
  }
}

export default function CustomOrderPage() {
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [items, setItems] = useState<OrderItemInput[]>([createEmptyItem()]);
  const [shippingAddress, setShippingAddress] = useState("");
  const [deliveryOption, setDeliveryOption] = useState<DeliveryOption>("standard");
  const [isPriority, setIsPriority] = useState(false);
  const [discountCode, setDiscountCode] = useState("");
  const [note, setNote] = useState("");
  const [useDirectorPreset, setUseDirectorPreset] = useState(false);
  const [presetType, setPresetType] = useState<PresetType>("quick");
  const [step, setStep] = useState<Step>(1);

  const [loadingProducts, setLoadingProducts] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<CustomOrderResultDto | null>(null);
  const [userId, setUserId] = useState(0);

  useEffect(() => {
    setUserId(getUserIdFromToken());

    api
      .get<ProductDto[]>("/products")
      .then((response) => setProducts(response.data))
      .catch(() => setError("Nu s-au putut incarca produsele."))
      .finally(() => setLoadingProducts(false));
  }, []);

  const normalizedItems = useMemo(() => {
    return items
      .map((item) => ({
        productId: Number(item.productId),
        quantity: Number(item.quantity),
      }))
      .filter((item) => item.productId > 0 && item.quantity > 0);
  }, [items]);

  const effectiveItems = useMemo(() => {
    if (useDirectorPreset && presetType === "quick") {
      return normalizedItems.slice(0, 1);
    }

    return normalizedItems;
  }, [normalizedItems, presetType, useDirectorPreset]);

  const effectiveDeliveryOption: DeliveryOption = useMemo(() => {
    if (!useDirectorPreset) {
      return deliveryOption;
    }

    return presetType === "priority" ? "express" : "standard";
  }, [deliveryOption, presetType, useDirectorPreset]);

  const effectivePriority = useMemo(() => {
    if (!useDirectorPreset) {
      return isPriority;
    }

    return presetType === "priority";
  }, [isPriority, presetType, useDirectorPreset]);

  const effectiveDiscountCode = useMemo(() => {
    if (useDirectorPreset) {
      return "";
    }

    return discountCode;
  }, [discountCode, useDirectorPreset]);

  const subtotal = useMemo(() => {
    const productMap = new Map(products.map((product) => [product.id, product]));

    return effectiveItems.reduce((total, item) => {
      const product = productMap.get(item.productId);
      if (!product) {
        return total;
      }

      return total + product.price * item.quantity;
    }, 0);
  }, [effectiveItems, products]);

  const discountAmount = useMemo(() => {
    const code = effectiveDiscountCode.trim().toUpperCase();

    if (!code) {
      return 0;
    }

    if (code === "SAVE10") {
      return subtotal * 0.1;
    }

    if (code === "SAVE50") {
      return Math.min(50, subtotal);
    }

    return 0;
  }, [effectiveDiscountCode, subtotal]);

  const shippingCost = useMemo(() => {
    const base = DELIVERY_BASE_COST[effectiveDeliveryOption];
    return base + (effectivePriority ? 20 : 0);
  }, [effectiveDeliveryOption, effectivePriority]);

  const estimatedTotal = useMemo(() => {
    return Math.max(0, subtotal - discountAmount + shippingCost);
  }, [discountAmount, shippingCost, subtotal]);

  const updateItem = (index: number, partial: Partial<OrderItemInput>) => {
    setItems((previous) =>
      previous.map((line, currentIndex) => (currentIndex === index ? { ...line, ...partial } : line))
    );
  };

  const addItem = () => {
    setItems((previous) => [...previous, createEmptyItem()]);
  };

  const removeItem = (index: number) => {
    setItems((previous) => {
      if (previous.length === 1) {
        return previous;
      }

      return previous.filter((_, currentIndex) => currentIndex !== index);
    });
  };

  const validateStep = (stepToValidate: Step): string | null => {
    if (stepToValidate === 1 && normalizedItems.length === 0) {
      return "Adauga cel putin un produs valid.";
    }

    if (stepToValidate === 2 && !shippingAddress.trim()) {
      return "Adresa de livrare este obligatorie.";
    }

    return null;
  };

  const goToNextStep = () => {
    const validationError = validateStep(step);
    if (validationError) {
      setError(validationError);
      return;
    }

    setError(null);
    setStep((previous) => {
      if (previous === 1) {
        return 2;
      }

      if (previous === 2) {
        return 3;
      }

      return previous;
    });
  };

  const goToPreviousStep = () => {
    setError(null);
    setStep((previous) => {
      if (previous === 3) {
        return 2;
      }

      if (previous === 2) {
        return 1;
      }

      return previous;
    });
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);
    setSuccess(null);

    const stepOneError = validateStep(1);
    const stepTwoError = validateStep(2);

    if (stepOneError) {
      setError(stepOneError);
      setStep(1);
      return;
    }

    if (stepTwoError) {
      setError(stepTwoError);
      setStep(2);
      return;
    }

    if (effectiveItems.length === 0) {
      setError("Nu exista produse valide pentru trimiterea comenzii.");
      return;
    }

    setSubmitting(true);

    try {
      const response = await api.post<CustomOrderResultDto>("/orders/custom", {
        userId,
        items: effectiveItems,
        shippingAddress: shippingAddress.trim(),
        discountCode: useDirectorPreset ? null : discountCode.trim() || null,
        isPriority: effectivePriority,
        note: useDirectorPreset ? null : note.trim() || null,
        deliveryOption: effectiveDeliveryOption,
        useDirectorPreset,
        presetType: useDirectorPreset ? presetType : null,
      });

      setSuccess(response.data);
    } catch (requestError: unknown) {
      if (requestError instanceof AxiosError) {
        const responseData = requestError.response?.data as ErrorMessageDto | undefined;
        setError(responseData?.message ?? "Nu s-a putut crea comanda custom.");
      } else {
        setError("Nu s-a putut crea comanda custom.");
      }
    } finally {
      setSubmitting(false);
    }
  };

  if (loadingProducts) {
    return <div className="p-8 text-center">Incarcare produse...</div>;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-cyan-50 to-lime-50 py-10 px-4">
      <div className="max-w-6xl mx-auto space-y-6">
        <header className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h1 className="text-3xl font-bold text-slate-900">Comanda custom (Builder)</h1>
            <p className="text-slate-600">Construieste comanda pas cu pas si foloseste preset-uri Director.</p>
          </div>
          <div className="flex gap-2">
            <Link href="/orders" className="px-4 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-100">
              Comenzile mele
            </Link>
            <Link href="/orders/templates" className="px-4 py-2 rounded-lg bg-slate-900 text-white hover:bg-slate-700">
              Template-uri
            </Link>
          </div>
        </header>

        <div className="grid lg:grid-cols-[2fr_1fr] gap-6">
          <form onSubmit={handleSubmit} className="bg-white rounded-2xl border border-slate-100 shadow-xl p-6 space-y-6">
            <section className="grid grid-cols-3 gap-2">
              {STEP_LABELS.map((item) => {
                const isActive = step === item.id;
                const isCompleted = step > item.id;

                return (
                  <div
                    key={item.id}
                    className={`rounded-xl px-3 py-2 text-center text-sm font-medium ${
                      isActive
                        ? "bg-cyan-600 text-white"
                        : isCompleted
                          ? "bg-emerald-100 text-emerald-800"
                          : "bg-slate-100 text-slate-500"
                    }`}
                  >
                    {item.id}. {item.label}
                  </div>
                );
              })}
            </section>

            {step === 1 && (
              <section className="space-y-3">
                <h2 className="text-xl font-semibold text-slate-900">Pas 1 - Produse</h2>
                <div className="space-y-3">
                  {items.map((item, index) => (
                    <div key={`item-${index}`} className="grid md:grid-cols-[1fr_120px_120px] gap-3 items-end">
                      <div>
                        <label className="block text-sm text-slate-700 mb-1">Produs</label>
                        <select
                          value={item.productId}
                          onChange={(event) => updateItem(index, { productId: event.target.value })}
                          className="w-full rounded-xl border border-slate-300 p-2.5"
                        >
                          <option value="">Alege produs</option>
                          {products.map((product) => (
                            <option key={product.id} value={product.id}>
                              {product.name} ({product.price.toFixed(2)} RON, stoc {product.stock})
                            </option>
                          ))}
                        </select>
                      </div>

                      <div>
                        <label className="block text-sm text-slate-700 mb-1">Cantitate</label>
                        <input
                          type="number"
                          min={1}
                          value={item.quantity}
                          onChange={(event) => updateItem(index, { quantity: Number(event.target.value) })}
                          className="w-full rounded-xl border border-slate-300 p-2.5"
                        />
                      </div>

                      <button
                        type="button"
                        onClick={() => removeItem(index)}
                        className="h-[42px] rounded-xl border border-rose-300 text-rose-700 hover:bg-rose-50"
                      >
                        Remove
                      </button>
                    </div>
                  ))}
                </div>

                <button
                  type="button"
                  onClick={addItem}
                  className="rounded-lg px-4 py-2 bg-cyan-600 text-white hover:bg-cyan-700"
                >
                  Add Product
                </button>
              </section>
            )}

            {step === 2 && (
              <section className="space-y-3">
                <h2 className="text-xl font-semibold text-slate-900">Pas 2 - Livrare</h2>
                <div className="space-y-2">
                  <label htmlFor="shippingAddress" className="font-medium text-slate-800">
                    Adresa de livrare
                  </label>
                  <textarea
                    id="shippingAddress"
                    rows={3}
                    value={shippingAddress}
                    onChange={(event) => setShippingAddress(event.target.value)}
                    className="w-full rounded-xl border border-slate-300 p-3"
                    placeholder="Ex: Strada Florilor 10, Brasov"
                  />
                </div>

                <div className="grid md:grid-cols-2 gap-3">
                  <div>
                    <label className="block font-medium text-slate-800 mb-1">Optiune livrare</label>
                    <select
                      value={deliveryOption}
                      onChange={(event) => setDeliveryOption(event.target.value as DeliveryOption)}
                      disabled={useDirectorPreset}
                      className="w-full rounded-xl border border-slate-300 p-2.5 disabled:bg-slate-100"
                    >
                      <option value="standard">Standard (15 RON)</option>
                      <option value="express">Express (45 RON)</option>
                      <option value="pickup">Pickup (0 RON)</option>
                    </select>
                  </div>

                  <label className="flex items-center gap-2 mt-7">
                    <input
                      type="checkbox"
                      checked={isPriority}
                      onChange={(event) => setIsPriority(event.target.checked)}
                      disabled={useDirectorPreset}
                    />
                    Prioritate (+20 RON)
                  </label>
                </div>
              </section>
            )}

            {step === 3 && (
              <section className="space-y-3">
                <h2 className="text-xl font-semibold text-slate-900">Pas 3 - Extras</h2>
                <div className="grid md:grid-cols-2 gap-3">
                  <div>
                    <label className="block text-sm text-slate-700 mb-1">Cod voucher</label>
                    <input
                      value={discountCode}
                      onChange={(event) => setDiscountCode(event.target.value)}
                      disabled={useDirectorPreset}
                      placeholder="SAVE10 / SAVE50"
                      className="w-full rounded-xl border border-slate-300 p-2.5 disabled:bg-slate-100"
                    />
                  </div>

                  <div>
                    <label className="block text-sm text-slate-700 mb-1">Preset Director</label>
                    <select
                      value={presetType}
                      onChange={(event) => setPresetType(event.target.value as PresetType)}
                      disabled={!useDirectorPreset}
                      className="w-full rounded-xl border border-slate-300 p-2.5 disabled:bg-slate-100"
                    >
                      <option value="quick">Quick</option>
                      <option value="priority">Priority</option>
                    </select>
                  </div>
                </div>

                <label className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    checked={useDirectorPreset}
                    onChange={(event) => setUseDirectorPreset(event.target.checked)}
                  />
                  Foloseste preset Director (ignora optiunile manuale)
                </label>

                <div>
                  <label className="block text-sm text-slate-700 mb-1">Nota pentru curier</label>
                  <textarea
                    rows={3}
                    value={note}
                    onChange={(event) => setNote(event.target.value)}
                    disabled={useDirectorPreset}
                    className="w-full rounded-xl border border-slate-300 p-3 disabled:bg-slate-100"
                    placeholder="Ex: Sunati inainte de livrare"
                  />
                </div>
              </section>
            )}

            {error && <p className="text-rose-600 font-medium">{error}</p>}

            <div className="flex flex-wrap gap-2">
              {step > 1 && (
                <button
                  type="button"
                  onClick={goToPreviousStep}
                  className="px-4 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-100"
                >
                  Inapoi
                </button>
              )}

              {step < 3 && (
                <button
                  type="button"
                  onClick={goToNextStep}
                  className="px-4 py-2 rounded-lg bg-slate-900 text-white hover:bg-slate-700"
                >
                  Urmatorul pas
                </button>
              )}

              {step === 3 && (
                <button
                  type="submit"
                  disabled={submitting}
                  className="px-5 py-2 rounded-lg bg-emerald-600 text-white hover:bg-emerald-700 disabled:opacity-50"
                >
                  {submitting ? "Se trimite..." : "Trimite comanda"}
                </button>
              )}
            </div>
          </form>

          <aside className="bg-white rounded-2xl border border-slate-100 shadow-xl p-5 space-y-3 h-fit">
            <h2 className="text-xl font-semibold text-slate-900">Preview total</h2>
            <p className="text-sm text-slate-600">Recalculare in timp real la fiecare schimbare.</p>

            <div className="text-sm space-y-1 text-slate-700">
              <p>Produse valide: {effectiveItems.length}</p>
              <p>Subtotal: {subtotal.toFixed(2)} RON</p>
              <p>Discount: -{discountAmount.toFixed(2)} RON</p>
              <p>Livrare: {shippingCost.toFixed(2)} RON</p>
              <p>Optiune livrare activa: {effectiveDeliveryOption}</p>
              <p>Prioritate: {effectivePriority ? "Da" : "Nu"}</p>
              <p className="pt-2 border-t border-slate-200 font-semibold text-slate-900">
                Total estimat: {estimatedTotal.toFixed(2)} RON
              </p>
            </div>

            {useDirectorPreset && (
              <p className="text-xs text-amber-700 bg-amber-50 border border-amber-200 rounded-lg px-2 py-1">
                Preset activ: {presetType === "quick" ? "Quick" : "Priority"}
              </p>
            )}
          </aside>
        </div>

        {success && (
          <section className="bg-emerald-50 border border-emerald-200 rounded-2xl p-5 space-y-2">
            <h3 className="text-xl font-semibold text-emerald-900">Comanda custom a fost creata</h3>
            <p>ID comanda: #{success.orderId}</p>
            <p>Status: {success.status}</p>
            <p>Total final: {success.finalTotal.toFixed(2)} RON</p>
            <p>Cost livrare: {success.shippingCost.toFixed(2)} RON</p>
            <p>Discount aplicat: {success.discountAmount.toFixed(2)} RON</p>
            <p>Data: {new Date(success.createdAt).toLocaleString()}</p>
            <Link href={`/orders/${success.orderId}`} className="inline-block text-emerald-700 underline">
              Vezi detalii comanda
            </Link>
          </section>
        )}
      </div>
    </div>
  );
}
