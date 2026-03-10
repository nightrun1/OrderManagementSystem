"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import api from "@/lib/api";

interface ProductDto {
  id: number;
  name: string;
  sku: string;
  price: number;
  stock: number;
  category: string;
}

export default function ProductsPage() {
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    api.get<ProductDto[]>("/products")
      .then((response) => setProducts(response.data))
      .catch(() => setError("Could not load products."))
      .finally(() => setLoading(false));
  }, []);

  const filteredProducts = products.filter(p => 
    p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    p.category.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-50">
      {/* Header */}
      <header className="border-b border-slate-200 bg-white/80 backdrop-blur-sm sticky top-0 z-10">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <Link href="/" className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-blue-600 to-indigo-600 rounded-lg flex items-center justify-center">
                <span className="text-white font-bold text-xl">O</span>
              </div>
              <h1 className="text-2xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">Products</h1>
            </Link>
            <div className="flex gap-3">
              <Link href="/dashboard" className="px-4 py-2 text-sm font-medium text-slate-700 hover:text-blue-600 transition-colors">Dashboard</Link>
              <Link href="/orders" className="px-4 py-2 text-sm font-medium text-slate-700 hover:text-blue-600 transition-colors">Orders</Link>
            </div>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Search Bar */}
        <div className="mb-8">
          <input
            type="text"
            placeholder="Search products by name or category..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full max-w-2xl px-6 py-4 border border-slate-300 rounded-2xl shadow-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
          />
        </div>

        {loading && (
          <div className="flex flex-col items-center justify-center py-16">
            <div className="w-16 h-16 border-4 border-blue-600 border-t-transparent rounded-full animate-spin mb-4"></div>
            <p className="text-slate-600">Loading products...</p>
          </div>
        )}

        {error && (
          <div className="bg-red-50 border border-red-200 rounded-2xl p-6 text-center">
            <p className="text-red-700 font-medium">{error}</p>
            <button onClick={() => window.location.reload()} className="mt-4 px-6 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors">
              Retry
            </button>
          </div>
        )}

        {!loading && !error && (
          <>
            <div className="mb-6 text-slate-600">
              Showing {filteredProducts.length} of {products.length} products
            </div>

            {filteredProducts.length === 0 ? (
              <div className="text-center py-16">
                <p className="text-xl text-slate-600 mb-4">No products found</p>
                <button onClick={() => setSearchTerm("")} className="text-blue-600 hover:underline">Clear search</button>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {filteredProducts.map((product) => (
                  <div key={product.id} className="group bg-white rounded-2xl shadow-md hover:shadow-xl transition-all duration-300 hover:-translate-y-1 border border-slate-100 overflow-hidden">
                    <div className="h-2 bg-gradient-to-r from-blue-600 to-indigo-600"></div>
                    <div className="p-6">
                      <div className="flex items-start justify-between mb-3">
                        <h3 className="text-xl font-bold text-slate-900 group-hover:text-blue-600 transition-colors">{product.name}</h3>
                        <span className="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-xs font-medium">{product.category}</span>
                      </div>
                      <p className="text-sm text-slate-500 mb-4">SKU: {product.sku}</p>
                      <div className="flex items-baseline gap-2 mb-4">
                        <p className="text-3xl font-bold text-slate-900">${product.price.toFixed(2)}</p>
                      </div>
                      <div className="flex items-center gap-2 mb-4">
                        <div className="flex-1 h-2 bg-slate-200 rounded-full overflow-hidden">
                          <div 
                            className="h-full bg-gradient-to-r from-green-500 to-emerald-500"
                            style={{width: `${Math.min(100, (product.stock / 100) * 100)}%`}}
                          ></div>
                        </div>
                        <span className="text-sm font-medium text-slate-600">{product.stock} left</span>
                      </div>
                      <button className="w-full py-3 px-4 bg-gradient-to-r from-blue-600 to-indigo-600 text-white font-medium rounded-xl hover:shadow-lg hover:scale-[1.02] active:scale-[0.98] transition-all disabled:opacity-50">
                        Add to Cart
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </>
        )}
      </main>
    </div>
  );
}
