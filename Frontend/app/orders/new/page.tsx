"use client";

import Link from "next/link";
import { FormEvent, useEffect, useMemo, useState } from "react";
import api from "@/lib/api";

type OrderType = "standard" | "express" | "bulk";

interface ProductDto {
  id: number;
  name: string;
  price: number;
  stock: number;
  category: string;
}

interface OrderItemInput {
  productId: string;
  quantity: number;
}

interface OrderItemDto {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}

interface CreatedOrderDto {
  id: number;
  status: string;
  totalAmount: number;
  shippingAddress: string;
  createdAt: string;
  orderType: string;
  shippingCost: number;
  items: OrderItemDto[];
}

const SHIPPING_PREVIEW: Record<OrderType, number> = {
  standard: 15,
  express: 45,
  bulk: 0,
};

const ORDER_TYPE_LABELS: Record<OrderType, string> = {
  standard: "Standard",
  express: "Express",
  bulk: "Bulk",
};

function createEmptyLine(): OrderItemInput {
  return { productId: "", quantity: 1 };
}

export default function NewOrderPage() {
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [shippingAddress, setShippingAddress] = useState("");
  const [orderType, setOrderType] = useState<OrderType>("standard");
  const [items, setItems] = useState<OrderItemInput[]>([createEmptyLine()]);
  const [submitting, setSubmitting] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [createdOrder, setCreatedOrder] = useState<CreatedOrderDto | null>(null);

  useEffect(() => {
    api
      .get<ProductDto[]>("/products")
      .then((response) => setProducts(response.data))
      .catch(() => setError("Nu s-au putut incarca produsele."))
      .finally(() => setLoading(false));
  }, []);

  const subtotal = useMemo(() => {
    const productsById = new Map(products.map((product) => [product.id, product]));
    return items.reduce((total, item) => {
      const productId = Number(item.productId);
      if (!productId || item.quantity <= 0) {
        return total;
      }

      const product = productsById.get(productId);
      if (!product) {
        return total;
      }

      return total + product.price * item.quantity;
    }, 0);
  }, [items, products]);

  const estimatedTotal = subtotal + SHIPPING_PREVIEW[orderType];

  const updateLine = (index: number, partial: Partial<OrderItemInput>) => {
    setItems((previous) =>
      previous.map((line, lineIndex) => (lineIndex === index ? { ...line, ...partial } : line))
    );
  };

  const addLine = () => {
    setItems((previous) => [...previous, createEmptyLine()]);
  };

  const removeLine = (index: number) => {
    setItems((previous) => {
      if (previous.length === 1) {
        return previous;
      }

      return previous.filter((_, lineIndex) => lineIndex !== index);
    });
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);
    setCreatedOrder(null);

    const normalizedItems = items
      .map((item) => ({
        productId: Number(item.productId),
        quantity: Number(item.quantity),
      }))
      .filter((item) => item.productId > 0 && item.quantity > 0);

    if (!shippingAddress.trim()) {
      setError("Adresa de livrare este obligatorie.");
      return;
    }

    if (normalizedItems.length === 0) {
      setError("Adauga cel putin un produs valid.");
      return;
    }

    setSubmitting(true);

    try {
      const response = await api.post<CreatedOrderDto>("/orders", {
        shippingAddress: shippingAddress.trim(),
        items: normalizedItems,
        orderType,
      });

      setCreatedOrder(response.data);
    } catch (requestError: any) {
      const message = requestError.response?.data?.message ?? "Nu s-a putut crea comanda.";
      setError(message);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <div className="p-8 text-center">Incarcare produse...</div>;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-cyan-50 to-teal-50 py-10 px-4">
      <div className="max-w-5xl mx-auto space-y-6">
        <header className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h1 className="text-3xl font-bold text-slate-900">Comanda noua</h1>
            <p className="text-slate-600 mt-1">Factory Method: alege Standard, Express sau Bulk.</p>
          </div>
          <div className="flex gap-2">
            <Link
              href="/orders"
              className="px-4 py-2 rounded-lg border border-slate-300 text-slate-700 hover:bg-slate-100 transition-colors"
            >
              Comenzile mele
            </Link>
            <Link
              href="/checkout"
              className="px-4 py-2 rounded-lg bg-slate-900 text-white hover:bg-slate-700 transition-colors"
            >
              Checkout
            </Link>
          </div>
        </header>

        <form onSubmit={handleSubmit} className="bg-white rounded-2xl shadow-xl border border-slate-100 p-6 space-y-6">
          <section className="space-y-3">
            <h2 className="text-xl font-semibold text-slate-900">Tip comanda</h2>
            <div className="grid sm:grid-cols-3 gap-3">
              {(Object.keys(ORDER_TYPE_LABELS) as OrderType[]).map((type) => (
                <label
                  key={type}
                  className={`rounded-xl border px-4 py-3 cursor-pointer transition-colors ${
                    orderType === type
                      ? "border-cyan-600 bg-cyan-50"
                      : "border-slate-200 hover:border-slate-400"
                  }`}
                >
                  <input
                    type="radio"
                    name="orderType"
                    value={type}
                    checked={orderType === type}
                    onChange={() => setOrderType(type)}
                    className="sr-only"
                  />
                  <p className="font-semibold text-slate-900">{ORDER_TYPE_LABELS[type]}</p>
                  <p className="text-sm text-slate-600">Transport: {SHIPPING_PREVIEW[type].toFixed(2)} RON</p>
                </label>
              ))}
            </div>
          </section>

          <section className="space-y-2">
            <label htmlFor="shippingAddress" className="font-medium text-slate-800">
              Adresa de livrare
            </label>
            <textarea
              id="shippingAddress"
              value={shippingAddress}
              onChange={(event) => setShippingAddress(event.target.value)}
              rows={3}
              className="w-full rounded-xl border border-slate-300 p-3 focus:outline-none focus:ring-2 focus:ring-cyan-500"
              placeholder="Ex: Strada Principala 12, Cluj-Napoca"
            />
          </section>

          <section className="space-y-3">
            <h2 className="text-xl font-semibold text-slate-900">Produse</h2>
            <div className="space-y-3">
              {items.map((line, index) => (
                <div key={`line-${index}`} className="grid md:grid-cols-[1fr_130px_120px] gap-3 items-end">
                  <div>
                    <label className="block text-sm text-slate-700 mb-1">Produs</label>
                    <select
                      value={line.productId}
                      onChange={(event) => updateLine(index, { productId: event.target.value })}
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
                      value={line.quantity}
                      onChange={(event) => updateLine(index, { quantity: Number(event.target.value) })}
                      className="w-full rounded-xl border border-slate-300 p-2.5"
                    />
                  </div>

                  <button
                    type="button"
                    onClick={() => removeLine(index)}
                    className="h-[42px] rounded-xl border border-rose-300 text-rose-700 hover:bg-rose-50 transition-colors"
                  >
                    Sterge linia
                  </button>
                </div>
              ))}
            </div>

            <button
              type="button"
              onClick={addLine}
              className="rounded-lg px-4 py-2 bg-cyan-600 text-white hover:bg-cyan-700 transition-colors"
            >
              Adauga produs
            </button>
          </section>

          <section className="rounded-xl border border-slate-200 bg-slate-50 p-4 text-sm text-slate-700 space-y-1">
            <p>Subtotal produse: {subtotal.toFixed(2)} RON</p>
            <p>Transport estimat: {SHIPPING_PREVIEW[orderType].toFixed(2)} RON</p>
            <p className="font-semibold text-slate-900">Total estimat: {estimatedTotal.toFixed(2)} RON</p>
          </section>

          {error && <p className="text-rose-600 font-medium">{error}</p>}

          <button
            type="submit"
            disabled={submitting}
            className="w-full rounded-xl py-3 bg-gradient-to-r from-cyan-600 to-teal-600 text-white font-semibold hover:opacity-90 disabled:opacity-50"
          >
            {submitting ? "Se trimite..." : "Creeaza comanda"}
          </button>
        </form>

        {createdOrder && (
          <section className="bg-emerald-50 border border-emerald-200 rounded-2xl p-5 space-y-2">
            <h3 className="text-xl font-semibold text-emerald-900">Comanda creata cu succes</h3>
            <p>Tip comanda: {createdOrder.orderType}</p>
            <p>Cost livrare (factory): {createdOrder.shippingCost.toFixed(2)} RON</p>
            <p>Status initial: {createdOrder.status}</p>
            <p>Total produse: {createdOrder.totalAmount.toFixed(2)} RON</p>
            <p>Data: {new Date(createdOrder.createdAt).toLocaleString()}</p>
            <Link href={`/orders/${createdOrder.id}`} className="inline-block mt-2 text-emerald-700 underline">
              Vezi detalii comanda #{createdOrder.id}
            </Link>
          </section>
        )}
      </div>
    </div>
  );
}
