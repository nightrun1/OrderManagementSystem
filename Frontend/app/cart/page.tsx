"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import api from "@/lib/api";

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

interface ProductDto {
  id: number;
  name: string;
  price: number;
  stock: number;
}

export default function CartPage() {
  const [cart, setCart] = useState<CartResponse>({ items: [], total: 0, itemCount: 0 });
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Add item form
  const [selectedProduct, setSelectedProduct] = useState<number>(0);
  const [quantity, setQuantity] = useState(1);

  // Add bundle form
  const [bundleName, setBundleName] = useState("");
  const [bundleDiscount, setBundleDiscount] = useState(10);
  const [bundleProducts, setBundleProducts] = useState<{ productId: number; quantity: number }[]>([]);

  useEffect(() => {
    Promise.all([
      api.get<CartResponse>("/cart"),
      api.get<ProductDto[]>("/products"),
    ])
      .then(([cartRes, prodRes]) => {
        setCart(cartRes.data);
        setProducts(prodRes.data);
        if (prodRes.data.length > 0) setSelectedProduct(prodRes.data[0].id);
      })
      .catch(() => setError("Nu s-a putut incarca cosul."))
      .finally(() => setLoading(false));
  }, []);

  const refreshCart = async () => {
    const res = await api.get<CartResponse>("/cart");
    setCart(res.data);
  };

  const addItem = async () => {
    try {
      await api.post("/cart/items", { productId: selectedProduct, quantity });
      await refreshCart();
    } catch {
      setError("Eroare la adaugarea produsului.");
    }
  };

  const addBundle = async () => {
    if (!bundleName || bundleProducts.length === 0) return;
    try {
      await api.post("/cart/bundles", {
        bundleId: Date.now(),
        bundleName,
        discountPercent: bundleDiscount,
        products: bundleProducts,
      });
      await refreshCart();
      setBundleName("");
      setBundleProducts([]);
    } catch {
      setError("Eroare la adaugarea bundle-ului.");
    }
  };

  const removeItem = async (itemId: number) => {
    try {
      await api.delete(`/cart/items/${itemId}`);
      await refreshCart();
    } catch {
      setError("Eroare la stergerea itemului.");
    }
  };

  const clearCart = async () => {
    try {
      await api.delete("/cart");
      setCart({ items: [], total: 0, itemCount: 0 });
    } catch {
      setError("Eroare la golirea cosului.");
    }
  };

  const addProductToBundle = () => {
    if (selectedProduct > 0) {
      setBundleProducts([...bundleProducts, { productId: selectedProduct, quantity: 1 }]);
    }
  };

  if (loading) return <div className="flex justify-center p-8">Incarcare...</div>;
  if (error) return <div className="text-red-500 p-4">{error}</div>;

  return (
    <div className="max-w-5xl mx-auto p-6">
      <header className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold">Cos de cumparaturi (Composite Pattern)</h1>
        <div className="flex gap-2">
          <Link href="/orders" className="bg-gray-600 hover:bg-gray-700 text-white px-3 py-2 rounded-lg">Comenzi</Link>
          <Link href="/shipping" className="bg-blue-600 hover:bg-blue-700 text-white px-3 py-2 rounded-lg">Livrare</Link>
          <Link href="/checkout" className="bg-emerald-600 hover:bg-emerald-700 text-white px-3 py-2 rounded-lg">Checkout</Link>
        </div>
      </header>

      {/* Adauga produs simplu */}
      <section className="bg-white rounded-xl shadow-md p-6 mb-6">
        <h2 className="text-lg font-semibold mb-3">Adauga produs</h2>
        <div className="flex gap-3 items-end flex-wrap">
          <select
            value={selectedProduct}
            onChange={(e) => setSelectedProduct(parseInt(e.target.value))}
            className="border rounded-lg px-3 py-2"
          >
            {products.map((p) => (
              <option key={p.id} value={p.id}>{p.name} — {p.price} RON</option>
            ))}
          </select>
          <input
            type="number" min={1} value={quantity}
            onChange={(e) => setQuantity(parseInt(e.target.value) || 1)}
            className="border rounded-lg px-3 py-2 w-20"
          />
          <button onClick={addItem} className="bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-2 rounded-lg">
            Adauga
          </button>
        </div>
      </section>

      {/* Adauga bundle */}
      <section className="bg-white rounded-xl shadow-md p-6 mb-6">
        <h2 className="text-lg font-semibold mb-3">Adauga bundle</h2>
        <div className="flex gap-3 items-end flex-wrap mb-3">
          <input
            type="text" placeholder="Nume bundle" value={bundleName}
            onChange={(e) => setBundleName(e.target.value)}
            className="border rounded-lg px-3 py-2"
          />
          <input
            type="number" placeholder="Discount %" value={bundleDiscount}
            onChange={(e) => setBundleDiscount(parseInt(e.target.value) || 0)}
            className="border rounded-lg px-3 py-2 w-28"
          />
          <button onClick={addProductToBundle} className="bg-blue-600 hover:bg-blue-700 text-white px-3 py-2 rounded-lg">
            + Produs in bundle
          </button>
          <button
            onClick={addBundle}
            disabled={!bundleName || bundleProducts.length === 0}
            className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-lg disabled:opacity-50"
          >
            Creeaza bundle
          </button>
        </div>
        {bundleProducts.length > 0 && (
          <div className="text-sm text-gray-600">
            Produse in bundle: {bundleProducts.map((bp, i) => {
              const p = products.find((pr) => pr.id === bp.productId);
              return <span key={i} className="inline-block bg-gray-100 rounded px-2 py-1 mr-1">{p?.name ?? `#${bp.productId}`} x{bp.quantity}</span>;
            })}
          </div>
        )}
      </section>

      {/* Cosul */}
      <section className="bg-white rounded-xl shadow-md p-6">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold">Cosul tau ({cart.itemCount} produse)</h2>
          <button onClick={clearCart} className="bg-red-500 hover:bg-red-600 text-white px-3 py-1 rounded-lg text-sm">
            Goleste cosul
          </button>
        </div>

        {cart.items.length === 0 ? (
          <p className="text-gray-500">Cosul este gol.</p>
        ) : (
          <div className="space-y-2">
            {cart.items.map((item, idx) => (
              <div
                key={idx}
                className="flex justify-between items-center py-2 border-b"
                style={{ paddingLeft: `${item.depth * 24}px` }}
              >
                <div className="flex items-center gap-2">
                  <span>{item.isBundle ? "📦" : item.depth > 0 ? "└──" : "📱"}</span>
                  <span className={item.isBundle ? "font-semibold" : ""}>{item.name}</span>
                  {!item.isBundle && <span className="text-gray-500">x{item.quantity}</span>}
                  {item.isBundle && <span className="text-purple-600 text-sm">(bundle)</span>}
                </div>
                <div className="flex items-center gap-3">
                  <span className="font-medium">{item.price.toFixed(2)} RON</span>
                  {item.depth === 0 && (
                    <button
                      onClick={() => removeItem(item.id)}
                      className="text-red-500 hover:text-red-700 text-sm"
                    >
                      Sterge
                    </button>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}

        <div className="mt-4 pt-4 border-t flex justify-between items-center">
          <span className="text-xl font-bold">Total: {cart.total.toFixed(2)} RON</span>
          <Link href="/checkout" className="bg-emerald-600 hover:bg-emerald-700 text-white px-6 py-2 rounded-lg">
            Checkout
          </Link>
        </div>
      </section>
    </div>
  );
}
