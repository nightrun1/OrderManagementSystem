"use client";

import { FormEvent, useEffect, useState } from "react";
import api from "@/lib/api";

interface ProductDto {
  id: number;
  name: string;
  sku: string;
  price: number;
  stock: number;
  category: string;
}

interface CreateProductRequest {
  name: string;
  sku: string;
  description: string;
  category: string;
  price: number;
  stock: number;
}

const emptyForm: CreateProductRequest = {
  name: "",
  sku: "",
  description: "",
  category: "",
  price: 0,
  stock: 0,
};

export default function AdminProductsPage() {
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [form, setForm] = useState<CreateProductRequest>(emptyForm);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadProducts = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await api.get<ProductDto[]>("/products");
      setProducts(response.data);
    } catch {
      setError("Nu s-au putut incarca produsele.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadProducts();
  }, []);

  const handleCreate = async (event: FormEvent) => {
    event.preventDefault();
    setSubmitting(true);
    setError(null);

    try {
      await api.post("/products", form);
      setForm(emptyForm);
      await loadProducts();
    } catch (err: any) {
      setError(err.response?.data?.message ?? "Crearea produsului a esuat.");
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    setError(null);
    try {
      await api.delete(`/products/${id}`);
      await loadProducts();
    } catch (err: any) {
      setError(err.response?.data?.message ?? "Stergerea produsului a esuat.");
    }
  };

  if (loading) {
    return <div className="flex justify-center p-8">Incarcare...</div>;
  }

  return (
    <div className="max-w-6xl mx-auto p-6 space-y-6">
      <h1 className="text-3xl font-bold">Admin Products</h1>

      <form onSubmit={handleCreate} className="bg-white rounded-xl shadow-md p-6 space-y-3">
        <h2 className="text-xl font-semibold">Adauga produs nou</h2>
        <input className="border border-gray-300 rounded-lg px-3 py-2 w-full" placeholder="Name" value={form.name} onChange={(e) => setForm((prev) => ({ ...prev, name: e.target.value }))} required />
        <input className="border border-gray-300 rounded-lg px-3 py-2 w-full" placeholder="SKU" value={form.sku} onChange={(e) => setForm((prev) => ({ ...prev, sku: e.target.value }))} required />
        <input className="border border-gray-300 rounded-lg px-3 py-2 w-full" placeholder="Description" value={form.description} onChange={(e) => setForm((prev) => ({ ...prev, description: e.target.value }))} required />
        <input className="border border-gray-300 rounded-lg px-3 py-2 w-full" placeholder="Category" value={form.category} onChange={(e) => setForm((prev) => ({ ...prev, category: e.target.value }))} required />
        <input className="border border-gray-300 rounded-lg px-3 py-2 w-full" placeholder="Price" type="number" step="0.01" value={form.price} onChange={(e) => setForm((prev) => ({ ...prev, price: Number(e.target.value) }))} required />
        <input className="border border-gray-300 rounded-lg px-3 py-2 w-full" placeholder="Stock" type="number" value={form.stock} onChange={(e) => setForm((prev) => ({ ...prev, stock: Number(e.target.value) }))} required />

        <button className="bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white px-4 py-2 rounded-lg" type="submit" disabled={submitting}>
          {submitting ? "Se proceseaza..." : "Create"}
        </button>
      </form>

      {error && <p className="text-red-500">{error}</p>}

      <div className="overflow-x-auto bg-white rounded-xl shadow-md">
        <table className="w-full border-collapse">
          <thead>
            <tr>
              <th className="bg-gray-100 px-4 py-2 text-left">ID</th>
              <th className="bg-gray-100 px-4 py-2 text-left">Name</th>
              <th className="bg-gray-100 px-4 py-2 text-left">SKU</th>
              <th className="bg-gray-100 px-4 py-2 text-left">Price</th>
              <th className="bg-gray-100 px-4 py-2 text-left">Stock</th>
              <th className="bg-gray-100 px-4 py-2 text-left">Action</th>
            </tr>
          </thead>
          <tbody>
            {products.map((product) => (
              <tr key={product.id}>
                <td className="px-4 py-2 border-b border-gray-200">{product.id}</td>
                <td className="px-4 py-2 border-b border-gray-200">{product.name}</td>
                <td className="px-4 py-2 border-b border-gray-200">{product.sku}</td>
                <td className="px-4 py-2 border-b border-gray-200">{product.price.toFixed(2)} RON</td>
                <td className="px-4 py-2 border-b border-gray-200">{product.stock}</td>
                <td className="px-4 py-2 border-b border-gray-200">
                  <button className="bg-red-600 hover:bg-red-700 text-white px-3 py-1 rounded-lg" onClick={() => handleDelete(product.id)}>
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
