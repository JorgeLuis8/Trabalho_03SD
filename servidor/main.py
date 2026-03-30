from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from sqlalchemy import Column, String, Float, DateTime, create_engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker
from datetime import datetime
import uvicorn

# --- CAMADA 3: PERSISTÊNCIA (SQLite) ---
DATABASE_URL = "sqlite:///./sensores.db"
engine = create_engine(DATABASE_URL, connect_args={"check_same_thread": False})
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
Base = declarative_base()

class LeituraDB(Base):
    __tablename__ = "leituras"
    id = Column(String, primary_key=True) # UUID do Cliente
    sensor_id = Column(String)
    temperatura = Column(Float)
    status_logico = Column(String)
    timestamp = Column(DateTime, default=datetime.utcnow)

# CORREÇÃO: O create_all é chamado via Base.metadata
Base.metadata.create_all(bind=engine)

# --- MODELO DE DADOS ---
class LeituraSchema(BaseModel):
    id: str
    sensor_id: str
    temperatura: float

# --- CAMADA 2: LÓGICA (FastAPI) ---
app = FastAPI()

# ADICIONADO: Configuração de CORS para o MAUI conseguir conectar
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], # Permite qualquer origem (ideal para desenvolvimento)
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.post("/leitura")
async def processar_leitura(data: LeituraSchema):
    db = SessionLocal()
    try:
        # 1. Idempotência (Mantido conforme requisito)
        registro = db.query(LeituraDB).filter(LeituraDB.id == data.id).first()
        if registro:
            return {"status_logico": registro.status_logico, "info": "Já processado"}

        # 2. Nova Lógica de Negócio (Escala Realista)
        temp = data.temperatura
        
        if temp < 0 or temp > 35:
            status = "Crítico"  # Congelamento ou Superaquecimento
        elif (0 <= temp <= 10) or (28 <= temp <= 35):
            status = "Alerta"   # Faixas de atenção (muito frio ou esquentando)
        else:
            status = "Normal"   # Faixa ideal (entre 11°C e 27°C)

        # 3. Persistência
        nova_leitura = LeituraDB(
            id=data.id,
            sensor_id=data.sensor_id,
            temperatura=temp,
            status_logico=status
        )
        db.add(nova_leitura)
        db.commit()

        return {"status_logico": status}
    
    except Exception as e:
        db.rollback()
        raise HTTPException(status_code=500, detail=f"Erro interno: {str(e)}")
    finally:
        db.close()

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=5000)